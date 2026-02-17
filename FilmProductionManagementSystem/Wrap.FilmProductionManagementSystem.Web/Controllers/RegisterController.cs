namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

using Wrap.Services.Core.Interface;
using Wrap.Services.Core.Utilities;

using Wrap.ViewModels.LoginAndRegistration;

[AllowAnonymous]
public class RegisterController(ILoginRegisterService registerService, 
                                ILogger<RegisterController> logger) : Controller
{
    private const string CrewDraftKey = "CrewDraft";
    private const string SuccessMessage = "Registration successful! Welcome to Wrap!";

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
            logger.LogError(e, "Exception occured while trying to build the crew draft");
            ModelState.AddModelError(string.Empty, e.Message);
        }
        
        return RedirectToAction(nameof(RegisterCrewStepTwo));
    }

    [HttpGet]
    public IActionResult RegisterCrewStepTwo()
    {
        CrewRegistrationStepOneDraft? draft = SessionJsonExtensions
            .GetJson<CrewRegistrationStepOneDraft>(HttpContext.Session, CrewDraftKey);
        
        if (draft is null)
            return RedirectToAction(nameof(RegisterCrewStepOne));
        
        return View(registerService.GetNewModelWithSkills());
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
            TempData["SuccessMessage"] = SuccessMessage;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to complete the crew registration");
            ModelState.AddModelError(string.Empty, e.Message);
            return View(model);
        }
        
        return RedirectToAction("Dashboard", "Home");
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

            TempData["SuccessMessage"] = SuccessMessage;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to complete the cast registration");
            ModelState.AddModelError(string.Empty, e.Message);
            return View(model);
        }
        
        return RedirectToAction("Dashboard", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        
        return View(new AccountLogInInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AccountLogInInputModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        try 
        {
            bool isCorrect = await registerService.IsUsernameAndPasswordCorrectAsync(model);
            if (!isCorrect)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            (bool, string) loginStatus = await registerService.LoginStatusAsync(model);

            if (loginStatus.Item1)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
            }

            switch (loginStatus)
            {
                case (true, "Crew"):
                case (true, "Cast"):
                    return RedirectToAction("Dashboard", "Home");
                case (false, "Crew"):
                    ModelState.AddModelError(string.Empty, "This account is not registered as Crew.");
                    break;
                case (false, "Cast"):
                    ModelState.AddModelError(string.Empty, "This account is not registered as Cast.");
                    break;
                default:
                    ModelState.AddModelError(string.Empty, "Please select a role.");
                    break;
            }
                
            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to login.");
            ModelState.AddModelError(string.Empty, $"Login failed: {e.Message}");
            return View(model);
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await registerService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    private bool IsSkillSelected(CrewRegistrationStepTwoInputModel model)
    {
        if (model.SelectedSkills.Count > 0)
            return false;
        
        ModelState.AddModelError(string.Empty, "Please select at least one skill.");
        registerService.GetSkills(model);
        return true;
    }
}