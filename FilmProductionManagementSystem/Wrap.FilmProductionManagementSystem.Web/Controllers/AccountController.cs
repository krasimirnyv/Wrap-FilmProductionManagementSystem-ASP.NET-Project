namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Wrap.Services.Core.Interface;
using Wrap.Services.Core.Utilities;

using Wrap.ViewModels.LoginAndRegistration;

public class AccountController(IWrapAccountService accountService, ILogger<AccountController> logger) : Controller
{
    private const string CrewDraftKey = "CrewDraft";

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
            CrewRegistrationStepOneDraft draft = await accountService.BuildCrewDraftAsync(model);
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
        
        return View(accountService.GetNewModelWithSkills());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterCrewStepTwo(CrewRegistrationStepTwoInputModel model)
    {
        if (ActionResult(model)) // bool method
            return View(model);
        
        try
        {
            CrewRegistrationStepOneDraft? draft = SessionJsonExtensions
                .GetJson<CrewRegistrationStepOneDraft>(HttpContext.Session, CrewDraftKey);
        
            if (draft is null)
                return RedirectToAction(nameof(RegisterCrewStepOne));
            
            IdentityResult identityResult = await accountService.CompleteCrewRegistrationAsync(draft, model.SelectedSkills);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                accountService.GetSkills(model);
                return View(model);
            }
        
            HttpContext.Session.Remove(CrewDraftKey);
            TempData["SuccessMessage"] = "Registration successful! Welcome to Wrap!";
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to complete the crew registration");
            ModelState.AddModelError(string.Empty, e.Message);
            return View(model);
        }
        
        return RedirectToAction("GeneralPage", "Home");
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
            IdentityResult identityResult = await accountService.CompleteCastRegistrationAsync(model);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration successful! Welcome to Wrap!";
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to complete the cast registration");
            ModelState.AddModelError(string.Empty, e.Message);
            return View(model);
        }
        
        return RedirectToAction("GeneralPage", "Home");
    }

    [HttpGet]
    public IActionResult Login() 
        => View(new AccountLogInInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AccountLogInInputModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        try 
        {
            bool isCorrect = await accountService.IsUsernameAndPasswordCorrectAsync(model);
            if (!isCorrect)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            (bool, string) logindStatus = await accountService.LoginStatusAsync(model);
            switch (logindStatus)
            {
                case (true, "Crew"):
                case (true, "Cast"):
                    return RedirectToAction("GeneralPage", "Home");
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
    public async Task<IActionResult> Logout()
    {
        await accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    private bool ActionResult(CrewRegistrationStepTwoInputModel model)
    {
        if (model.SelectedSkills.Count > 0)
            return false;
        
        ModelState.AddModelError(string.Empty, "Please select at least one skill.");
        accountService.GetSkills(model);
        return true;
    }
}