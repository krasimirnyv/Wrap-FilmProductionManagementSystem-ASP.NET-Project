namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

using Wrap.Services.Core.Interfaces;
using Wrap.Services.Core.Utilities;
using Wrap.Services.Models.LoginAndRegistration;
using Wrap.ViewModels.LoginAndRegistration;

using static Wrap.GCommon.ApplicationConstants;
using static Wrap.GCommon.OutputMessages;
using static Wrap.GCommon.OutputMessages.Register;

[AllowAnonymous]
public class LoginRegisterController(ILoginRegisterService registerService, 
                                     ILogger<LoginRegisterController> logger) : Controller
{
    [HttpGet]
    public IActionResult RegisterCrewStepOne()
        => View(new CrewRegistrationStepOneInputModel());

    [HttpPost]
    public async Task<IActionResult> RegisterCrewStepOne(CrewRegistrationStepOneInputModel inputModel)
    {
        if (!ModelState.IsValid)
            return View(inputModel);

        try
        {
            CrewRegistrationStepOneDto dto = MapToCrewRegistrationStepOneDtoFromInputModel(inputModel);
            
            CrewRegistrationDraftDto? draft = await registerService.BuildCrewDraftAsync(dto);
            if (draft is null)
                return View(nameof(BadRequest), string.Format(ErrorBuildingCrewDraft));

            SessionJsonExtensions.SetJson(HttpContext.Session, CrewDraftKey, draft);

            return RedirectToAction(nameof(RegisterCrewStepTwo));
        }
        catch (NotSupportedException nse)
        {
            logger.LogError(nse, string.Format(ErrorSavingTheImage, CreatingMessage, nse.Message));
            ModelState.AddModelError(string.Empty, string.Format(ErrorSavingTheImage, CreatingMessage, nse.Message));
            return View(inputModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ExceptionBuildingCrewDraft, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ExceptionBuildingCrewDraft, e.Message));
            return View(inputModel);
        }
    }

    [HttpGet]
    public IActionResult RegisterCrewStepTwo()
    {
        CrewRegistrationDraftDto? draft = SessionJsonExtensions
            .GetJson<CrewRegistrationDraftDto>(HttpContext.Session, CrewDraftKey);

        if (draft is null)
        {
            TempData[ErrorTempDateKey] = ErrorFoundingCrewDraft;
            return RedirectToAction(nameof(RegisterCrewStepOne));
        }

        CrewRegistrationStepTwoDto dto = registerService.GetNewModelWithSkills();
        CrewRegistrationStepTwoInputModel inputModel = MapToCrewRegistrationStepTwoInputModelFromDto(dto);
            
        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCrewStepTwo(CrewRegistrationStepTwoInputModel inputModel)
    {
        CrewRegistrationStepTwoDto dto = MapToCrewRegistrationStepTwoDtoFromInputModel(inputModel);
        if (IsSkillSelected(dto))
            return View(inputModel);
        
        CrewRegistrationDraftDto? draft = SessionJsonExtensions
            .GetJson<CrewRegistrationDraftDto>(HttpContext.Session, CrewDraftKey);
        
        if (draft is null)
        {
            TempData[ErrorTempDateKey] = ErrorFoundingCrewDraft;
            return RedirectToAction(nameof(RegisterCrewStepOne));
        }
        
        CrewRegistrationCompleteDto registrationDto = MapToCrewRegistrationCompleteDtoFromDraftAndInputModel(inputModel, draft);
        
        try
        {
            IdentityResult result = await registerService.CompleteRegistrationAsync(registrationDto);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                registerService.GetSkills(registrationDto);
                return View(inputModel);
            }
        
            HttpContext.Session.Remove(CrewDraftKey);
            TempData[SuccessTempDataKey] = SuccessMessage;
            
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ExceptionCompleteRegistrationOfCrewMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ExceptionCompleteRegistrationOfCrewMessage, e.Message);
            return RedirectToAction(nameof(Index), nameof(Home));
        }
    }

    [HttpGet]
    public IActionResult RegisterCast()
        => View(new CastRegistrationInputModel());
    
    [HttpPost]
    public async Task<IActionResult> RegisterCast(CastRegistrationInputModel inputModel)
    {
        if (!ModelState.IsValid)
            return View(inputModel);

        CastRegistrationDto dto = MapToCastRegistrationDtoFromInputModel(inputModel);
            
        try
        {
            IdentityResult result = await registerService.CompleteRegistrationAsync(dto);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            
                return View(inputModel);
            }

            TempData[SuccessTempDataKey] = SuccessMessage;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (NotSupportedException nse)
        {
            logger.LogError(nse, string.Format(ErrorSavingTheImage, CreatingMessage, nse.Message));
            ModelState.AddModelError(string.Empty, string.Format(ErrorSavingTheImage, CreatingMessage, nse.Message));
            return View(inputModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ExceptionCompleteRegistrationOfCastMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ExceptionCompleteRegistrationOfCastMessage, e.Message);
            return RedirectToAction(nameof(Index), nameof(Home));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl)
    {
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        
        return View(new AccountLogInInputModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(AccountLogInInputModel inputModel, string? returnUrl)
    {
        if (!ModelState.IsValid)
            return View(inputModel);

        LoginRequestDto dto = MapToLoginRequestDtoFromInputModel(inputModel);

        try
        {
            LoginStatusDto loginStatus = await registerService.LoginStatusAsync(dto);

            if (loginStatus is { IsSucceeded: true, Role: CrewString }
                            or { IsSucceeded: true, Role: CastString })
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Dashboard", "Home");
            }

            AddModelErrorForSpecificCase(loginStatus);

            return View(inputModel);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ExceptionLogin, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ExceptionLogin, e.Message));
            return View(inputModel);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        await registerService.LogoutAsync();

        return RedirectToAction(nameof(Index), "Home");
    }
    
    private static CrewRegistrationStepOneDto MapToCrewRegistrationStepOneDtoFromInputModel(CrewRegistrationStepOneInputModel inputModel)
    {
        CrewRegistrationStepOneDto dto = new CrewRegistrationStepOneDto
        {
            UserName = inputModel.UserName,
            Email = inputModel.Email,
            PhoneNumber = inputModel.PhoneNumber,
            Password = inputModel.Password,
            FirstName = inputModel.FirstName,
            LastName = inputModel.LastName,
            Nickname = inputModel.Nickname,
            ProfilePicture = inputModel.ProfilePicture,
            Biography = inputModel.Biography
        };
        
        return dto;
    }
    
    private static CrewRegistrationStepTwoInputModel MapToCrewRegistrationStepTwoInputModelFromDto(CrewRegistrationStepTwoDto dto)
    {
        CrewRegistrationStepTwoInputModel inputModel = new CrewRegistrationStepTwoInputModel
        {
            SelectedSkills = dto.SelectedSkills,
            SkillsByDepartment = dto.SkillsByDepartment
        };
        
        return inputModel;
    }
    
    private static CrewRegistrationStepTwoDto MapToCrewRegistrationStepTwoDtoFromInputModel(CrewRegistrationStepTwoInputModel inputModel)
    {
        CrewRegistrationStepTwoDto dto = new CrewRegistrationStepTwoDto
        {
            SelectedSkills = inputModel.SelectedSkills,
            SkillsByDepartment = inputModel.SkillsByDepartment
        };
        
        return dto;
    }
    
    private static CrewRegistrationCompleteDto MapToCrewRegistrationCompleteDtoFromDraftAndInputModel(CrewRegistrationStepTwoInputModel inputModel, CrewRegistrationDraftDto draft)
    {
        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto
        {
            Draft = draft,
            SkillNumbers = inputModel.SelectedSkills
        };
        
        return dto;
    }
    
    private static CastRegistrationDto MapToCastRegistrationDtoFromInputModel(CastRegistrationInputModel inputModel)
    {
        CastRegistrationDto dto = new CastRegistrationDto
        {
            UserName = inputModel.UserName,
            Email = inputModel.Email,
            PhoneNumber = inputModel.PhoneNumber,
            Password = inputModel.Password,
            FirstName = inputModel.FirstName,
            LastName = inputModel.LastName,
            Nickname = inputModel.Nickname,
            BirthDate = inputModel.BirthDate,
            Gender = inputModel.Gender,
            ProfilePicture = inputModel.ProfilePicture,
            Biography = inputModel.Biography,
        };
        
        return dto;
    }
    
    private static LoginRequestDto MapToLoginRequestDtoFromInputModel(AccountLogInInputModel inputModel)
    {
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = inputModel.UserName,
            Password = inputModel.Password,
            Role = inputModel.Role,
            RememberMe = inputModel.RememberMe
        };
        
        return dto;
    }
    
    private bool IsSkillSelected(CrewRegistrationStepTwoDto dto)
    {
        if (dto.SelectedSkills.Count > 0)
            return false;
        
        ModelState.AddModelError(string.Empty, NoSelectedSkills);
        registerService.GetSkills(dto);
        
        return true;
    }
    
    private void AddModelErrorForSpecificCase(LoginStatusDto loginStatus)
    {
        if (loginStatus is { IsSucceeded: false, Role: CrewString })
            ModelState.AddModelError(string.Empty, NotRegisteredAsCrew);
        else if (loginStatus is { IsSucceeded: false, Role: CastString })
            ModelState.AddModelError(string.Empty, NotRegisteredAsCast);
        else if (loginStatus is { IsSucceeded: false, Role: EmptyString })
            ModelState.AddModelError(string.Empty, InvalidUsernameOrPassword);
        else
            ModelState.AddModelError(string.Empty, NotSelectedRole);
    }
}