namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

using Wrap.Services.Core.Interface;
using Wrap.ViewModels.Profile;

using static Wrap.GCommon.OutputMessages.Profile;
using static Wrap.GCommon.ApplicationConstants;

public class ProfileController(IProfileService profileService, 
                               ILogger<ProfileController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            bool isCrew = await profileService.IsUserCrewAsync(username);
            if (isCrew)
                return RedirectToAction(nameof(FilmmakerProfile), new { username });

            bool isCast = await profileService.IsUserCastAsync(username);
            if (isCast)
                return RedirectToAction(nameof(ActorProfile), new { username });

            logger.LogWarning(string.Format(UserNotFoundMessage, username));
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorFindingUserMessage, e.Message));
            return View(nameof(BadRequest), UserNotIdentifiedMessage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> FilmmakerProfile(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            CrewProfileViewModel profile = await profileService.GetCrewProfileDataAsync(username);
            return View(profile);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(DataExceptionMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingProfileMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ActorProfile(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            CastProfileViewModel profile = await profileService.GetCastProfileDataAsync(username);
            return View(profile);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(DataExceptionMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingProfileMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning(UsernameIsNullOrEmptyMessage);
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            bool isCrew = await profileService.IsUserCrewAsync(username);
            if (isCrew)
                return RedirectToAction(nameof(EditFilmmaker), new { username });

            bool isCast = await profileService.IsUserCastAsync(username);
            if (isCast)
                return RedirectToAction(nameof(EditActor), new { username });
            
            logger.LogWarning(string.Format(UserNotFoundMessage, username));
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorFindingUserMessage, e.Message));
            return View(nameof(BadRequest), UserNotIdentifiedMessage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditFilmmaker(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning($"{nameof(EditFilmmaker)} GET: {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            EditCrewProfileViewModel model = await profileService.GetEditCrewProfileAsync(username);
            return View("EditCrewProfile", model);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorLoadingEditorMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingEditorMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditFilmmaker(EditCrewProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View("EditCrewProfile", model);

        string? username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning($"{nameof(EditFilmmaker)} POST: {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            await profileService.UpdateCrewProfileAsync(username, model);

            TempData[SuccessTempDataKey] = UpdateProfileSuccessMessage;
            return RedirectToAction(nameof(FilmmakerProfile));
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorUpdatingProfile, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ErrorUpdatingProfile, e.Message));
            return View("EditCrewProfile", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditActor(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning($"{nameof(EditActor)} GET: {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            EditCastProfileViewModel model = await profileService.GetEditCastProfileAsync(username);
            return View("EditCastProfile", model);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorLoadingEditorMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingEditorMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditActor(EditCastProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View("EditCastProfile", model);

        string? username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning($"{nameof(EditActor)} POST: {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            await profileService.UpdateCastProfileAsync(username, model);

            TempData[SuccessTempDataKey] = UpdateProfileSuccessMessage;
            return RedirectToAction(nameof(ActorProfile));
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorUpdatingProfile, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ErrorUpdatingProfile, e.Message));
            return View("EditCastProfile", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditSkills(string? username)
    {
        username ??= GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning($"{nameof(EditSkills)} GET: {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }
        
        try
        {
            EditSkillsViewModel model = await profileService.GetEditSkillsAsync(username);
            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorLoadingSkillsMessage, e.Message));
            TempData[ErrorTempDateKey] = string.Format(ErrorLoadingSkillsMessage, e.Message);
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditSkills(EditSkillsViewModel model)
    {
        string? username = GetUsername();
        if (string.IsNullOrEmpty(username))
        {
            logger.LogWarning($"{nameof(EditSkills)} POST: {UsernameIsNullOrEmptyMessage}");
            return View(nameof(NotFound), UserNotIdentifiedMessage);
        }

        if (!ModelState.IsValid)
        {
            EditSkillsViewModel currentModel = await profileService.GetEditSkillsAsync(username);
            model.CurrentSkills = currentModel.CurrentSkills;
            return View(model);
        }

        try
        {
            await profileService.UpdateSkillsAsync(username, model);

            TempData[SuccessTempDataKey] = UpdateSkillsSuccessMessage;
            return RedirectToAction(nameof(FilmmakerProfile));
        }
        catch (Exception e)
        {
            logger.LogError(e, string.Format(ErrorUpdatingProfile, e.Message));
            ModelState.AddModelError(string.Empty, string.Format(ErrorUpdatingProfile, e.Message));
            return View(model);
        }
    }
}