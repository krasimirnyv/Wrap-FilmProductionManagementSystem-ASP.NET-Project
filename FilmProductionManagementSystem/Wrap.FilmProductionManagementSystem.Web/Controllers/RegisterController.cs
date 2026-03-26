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
public class RegisterController(ILoginRegisterService registerService, 
                                ILogger<RegisterController> logger) : Controller
{
    [HttpGet]
    public IActionResult RegisterCrewStepOne()
        => View(new CrewRegistrationStepOneInputModel());

    [HttpPost]
    public async Task<IActionResult> RegisterCrewStepOne(CrewRegistrationStepOneInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            CrewRegistrationDraftDto? draft = await registerService.BuildCrewDraftAsync(model);
            if (draft is null)
                return View(nameof(BadRequest), string.Format(ErrorBuildingCrewDraft));

            SessionJsonExtensions.SetJson(HttpContext.Session, CrewDraftKey, draft);

            return RedirectToAction(nameof(RegisterCrewStepTwo));
        }
        catch (NotSupportedException nse)
        {
            logger.LogError(nse, string.Format(ErrorSavingTheImage, CreatingMessage, nse.Message));
            ModelState.AddModelError(string.Empty, string.Format(ErrorSavingTheImage, CreatingMessage, nse.Message));
            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ExceptionBuildingCrewDraft, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ExceptionBuildingCrewDraft, e.Message));
            return View(model);
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

        CrewRegistrationStepTwoInputModel inputModel = registerService.GetNewModelWithSkills();
        
        return View(inputModel);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCrewStepTwo(CrewRegistrationStepTwoInputModel model)
    {
        if (IsSkillSelected(model))
            return View(model);
        
        CrewRegistrationDraftDto? draft = SessionJsonExtensions
            .GetJson<CrewRegistrationDraftDto>(HttpContext.Session, CrewDraftKey);
        
        if (draft is null)
        {
            TempData[ErrorTempDateKey] = ErrorFoundingCrewDraft;
            return RedirectToAction(nameof(RegisterCrewStepOne));
        }
        
        CrewRegistrationCompleteDto dto = MapToCrewRegistrationCompleteDtoFromDraftAndModel(model, draft);
        
        try
        {
            IdentityResult result = await registerService.CompleteCrewRegistrationAsync(dto);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                registerService.GetSkills(model);
                return View(model);
            }
        
            HttpContext.Session.Remove(CrewDraftKey);
            TempData[SuccessTempDataKey] = SuccessMessage;
            
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ExceptionCompleteRegistrationOfCrewMessage, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ExceptionCompleteRegistrationOfCrewMessage, e.Message));
            registerService.GetSkills(model);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult RegisterCast()
        => View(new CastRegistrationInputModel());
    
    [HttpPost]
    public async Task<IActionResult> RegisterCast(CastRegistrationInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        CastRegistrationDto dto = MapToCastRegistrationDtoFromInputModel(model);
            
        try
        {
            IdentityResult result = await registerService.CompleteCastRegistrationAsync(dto);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            
                return View(model);
            }

            TempData[SuccessTempDataKey] = SuccessMessage;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (NotSupportedException nse)
        {
            logger.LogError(nse, string.Format(ErrorSavingTheImage, CreatingMessage, nse.Message));
            ModelState.AddModelError(string.Empty, string.Format(ErrorSavingTheImage, CreatingMessage, nse.Message));
            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ExceptionCompleteRegistrationOfCastMessage, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ExceptionCompleteRegistrationOfCastMessage, e.Message));
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl)
    {
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        
        return View(new AccountLogInInputModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(AccountLogInInputModel model, string? returnUrl)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        LoginRequestDto dto = MapToLoginRequestDtoFromInputModel(model);
        
        try
        {
            (bool Succeeded, string Role) loginStatus = await registerService.LoginStatusAsync(dto);
            if (loginStatus == (true, CrewString) || loginStatus == (true, CastString))
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                
                return RedirectToAction("Dashboard", "Home");
            }

            if (loginStatus is (false, CrewString))
                ModelState.AddModelError(string.Empty, NotRegisteredAsCrew);
            else if (loginStatus is (false, CastString))
                ModelState.AddModelError(string.Empty, NotRegisteredAsCast);
            else if (loginStatus is (false, EmptyString))
                ModelState.AddModelError(string.Empty, InvalidUsernameOrPassword);
            else
                ModelState.AddModelError(string.Empty, NotSelectedRole);

            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ExceptionLogin, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ExceptionLogin, e.Message));
            return View(model);
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
    
    
    private static CrewRegistrationCompleteDto MapToCrewRegistrationCompleteDtoFromDraftAndModel(CrewRegistrationStepTwoInputModel model, CrewRegistrationDraftDto draft)
    {
        CrewRegistrationCompleteDto dto = new CrewRegistrationCompleteDto
        {
            Draft = draft,
            SkillNumbers = model.SelectedSkills
        };
        
        return dto;
    }
    
    private static CastRegistrationDto MapToCastRegistrationDtoFromInputModel(CastRegistrationInputModel model)
    {
        CastRegistrationDto dto = new CastRegistrationDto
        {
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Nickname = model.Nickname,
            BirthDate = model.BirthDate,
            Gender = model.Gender,
            ProfilePicture = model.ProfilePicture,
            Biography = model.Biography,
        };
        
        return dto;
    }
    
    private static LoginRequestDto MapToLoginRequestDtoFromInputModel(AccountLogInInputModel model)
    {
        LoginRequestDto dto = new LoginRequestDto
        {
            UserName = model.UserName,
            Password = model.Password,
            Role = model.Role,
            RememberMe = model.RememberMe
        };
        
        return dto;
    }
    
    private bool IsSkillSelected(CrewRegistrationStepTwoInputModel model)
    {
        if (model.SelectedSkills.Count > 0)
            return false;
        
        ModelState.AddModelError(string.Empty, NoSelectedSkills);
        registerService.GetSkills(model);
        
        return true;
    }
}