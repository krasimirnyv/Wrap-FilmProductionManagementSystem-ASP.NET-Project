namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

using Wrap.Services.Core.Interface;
using Wrap.Services.Core.Utilities;
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterCrewStepOne(CrewRegistrationStepOneInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            CrewRegistrationStepOneDraft draft = await registerService.BuildCrewDraftAsync(model);
            SessionJsonExtensions.SetJson(HttpContext.Session, CrewDraftKey, draft);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorBuildingCrewDraft, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ErrorBuildingCrewDraft, e.Message));
            return View(model);
        }
        
        return RedirectToAction(nameof(RegisterCrewStepTwo));
    }

    [HttpGet]
    public IActionResult RegisterCrewStepTwo()
    {
        CrewRegistrationStepOneDraft? draft = SessionJsonExtensions
            .GetJson<CrewRegistrationStepOneDraft>(HttpContext.Session, CrewDraftKey);

        if (draft is null)
        {
            TempData[ErrorTempDateKey] = ErrorFoundingCrewDraft;
            return RedirectToAction(nameof(RegisterCrewStepOne));
        }

        CrewRegistrationStepTwoInputModel inputModel = registerService.GetNewModelWithSkills();
        return View(inputModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterCrewStepTwo(CrewRegistrationStepTwoInputModel model)
    {
        if (IsSkillSelected(model)) // bool method
            return View(model);
        
        try
        {
            CrewRegistrationStepOneDraft? draft = SessionJsonExtensions
                .GetJson<CrewRegistrationStepOneDraft>(HttpContext.Session, CrewDraftKey);
        
            if (draft is null)
                return RedirectToAction(nameof(RegisterCrewStepOne));
            
            IdentityResult identityResult = await registerService.CompleteCrewRegistrationAsync(draft, model.SelectedSkills);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
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
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult RegisterCast()
        => View(new CastRegistrationInputModel());
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterCast(CastRegistrationInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            IdentityResult identityResult = await registerService.CompleteCastRegistrationAsync(model);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            
                return View(model);
            }

            TempData[SuccessTempDataKey] = SuccessMessage;
            return RedirectToAction("Dashboard", "Home");
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
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        
        return View(new AccountLogInInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AccountLogInInputModel model, string? returnUrl)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        try 
        {
            (bool, string) loginStatus = await registerService.LoginStatusAsync(model);

            if (loginStatus.Item1)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
            }

            switch (loginStatus)
            {
                case (true, CrewString):
                case (true, CastString):
                    return RedirectToAction("Dashboard", "Home");
                case (false, CrewString):
                    ModelState.AddModelError(string.Empty, NotRegisteredAsCrew);
                    break;
                case (false, CastString):
                    ModelState.AddModelError(string.Empty, NotRegisteredAsCast);
                    break;
                case (false, EmptyString):
                    ModelState.AddModelError(string.Empty, InvalidUsernameOrPassword);
                    break;
                default:
                    ModelState.AddModelError(string.Empty, NotSelectedRole);
                    break;
            }
                
            await registerService.LogoutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            
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
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        await registerService.LogoutAsync();

        return RedirectToAction(nameof(Index), "Home");
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