namespace FilmProductionManagementSystem.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Wrap.Services.Core.Interface;

using Wrap.ViewModels.Profile;

[Authorize]
public class ProfileController(IProfileService profileService, 
                               ILogger<ProfileController> logger) : Controller
{
    private const string NotFoundMessage = "Invalid user identifier.";
    private const string UsernameIsNullOrEmptyMessage = "Username is null or empty.";

    [HttpGet]
    public async Task<IActionResult> Index(string? username)
    {
        try
        {
            username ??= User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning(UsernameIsNullOrEmptyMessage);
                return View("NotFound", NotFoundMessage);
            }

            bool isCrew = await profileService.IsUserCrewAsync(username);
            if (isCrew)
                return RedirectToAction("FilmmakerProfile", new { username });

            bool isCast = await profileService.IsUserCastAsync(username);
            if (isCast)
                return RedirectToAction("ActorProfile", new { username });

            logger.LogWarning("User not found as crew or cast: {Username}", username);
            return View("NotFound", NotFoundMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to find your profile. {ExMessage}", e.Message);
            return View("NotFound", NotFoundMessage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> FilmmakerProfile(string? username = null)
    {
        try
        {
            username ??= User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning(UsernameIsNullOrEmptyMessage);
                return View("NotFound", NotFoundMessage);
            }

            CrewProfileViewModel profile = await profileService.GetCrewProfileDataAsync(username);
            return View(profile);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to get data of your profile an a filmmaker. {ExMessage}", e.Message);
            TempData["Error"] = $"Error loading profile: {e.Message}";
            return RedirectToAction("Dashboard", "Home");
        }

    }

    [HttpGet]
    public async Task<IActionResult> ActorProfile(string? username = null)
    {
        try
        {
            username ??= User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning(UsernameIsNullOrEmptyMessage);
                return View("NotFound", NotFoundMessage);
            }

            CastProfileViewModel profile = await profileService.GetCastProfileDataAsync(username);
            return View(profile);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to get data of your profile as an actor. {ExMessage}",
                e.Message);
            TempData["Error"] = $"Error loading profile: {e.Message}";
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? username)
    {
        try
        {
            username ??= User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning(UsernameIsNullOrEmptyMessage);
                return View("NotFound", NotFoundMessage);
            }

            bool isCrew = await profileService.IsUserCrewAsync(username);
            if (isCrew)
                return RedirectToAction("EditFilmmaker", new { username });

            bool isCast = await profileService.IsUserCastAsync(username);
            if (isCast)
                return RedirectToAction("EditActor", new { username });

            logger.LogWarning("User not found as crew or cast: {Username}", username);
            return View("NotFound", NotFoundMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while trying to find your profile. {ExMessage}", e.Message);
            return View("NotFound", NotFoundMessage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditFilmmaker(string? username = null)
    {
        try
        {
            username ??= User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning($"EditFilmmaker: {UsernameIsNullOrEmptyMessage}");
                return View("NotFound", NotFoundMessage);
            }

            EditCrewProfileViewModel model = await profileService.GetEditCrewProfileAsync(username);
            return View("EditCrewProfile", model);
        }
        catch (Exception e)
        {
            logger.LogError(e, "EditFilmmaker exception: {ExMessage}", e.Message);
            TempData["Error"] = $"Error loading editor for profile: {e.Message}";
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFilmmaker(EditCrewProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View("EditCrewProfile", model);

        try
        {
            string? username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning($"EditFilmmaker POST: {UsernameIsNullOrEmptyMessage}");
                return View("NotFound", NotFoundMessage);
            }

            await profileService.UpdateCrewProfileAsync(username, model);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("FilmmakerProfile");
        }
        catch (Exception e)
        {
            logger.LogError(e, "EditFilmmaker POST exception: {ExMessage}", e.Message);
            ModelState.AddModelError("", $"Error updating profile: {e.Message}");
            return View("EditCrewProfile", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditActor(string? username = null)
    {
        try
        {
            username ??= User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning($"EditActor: {UsernameIsNullOrEmptyMessage}");
                return View("NotFound", NotFoundMessage);
            }

            EditCastProfileViewModel model = await profileService.GetEditCastProfileAsync(username);
            return View("EditCastProfile", model);
        }
        catch (Exception e)
        {
            logger.LogError(e, "EditActor exception: {ExMessage}", e.Message);
            TempData["Error"] = $"Error loading editor for profile: {e.Message}";
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditActor(EditCastProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View("EditCastProfile", model);

        try
        {
            string? username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning($"EditActor POST: {UsernameIsNullOrEmptyMessage}");
                return View("NotFound", NotFoundMessage);
            }

            await profileService.UpdateCastProfileAsync(username, model);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("ActorProfile");
        }
        catch (Exception e)
        {
            logger.LogError(e, "EditActor POST exception: {ExMessage}", e.Message);
            ModelState.AddModelError("", $"Error updating profile: {e.Message}");
            return View("EditCastProfile", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditSkills(string? username)
    {
        try
        {
            username ??= User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning($"EditSkills: {UsernameIsNullOrEmptyMessage}");
                return View("NotFound", NotFoundMessage);
            }

            EditSkillsViewModel model = await profileService.GetEditSkillsAsync(username);
            return View(model);
        }
        catch (Exception e)
        {
            logger.LogError(e, "EditSkills exception: {ExMessage}", e.Message);
            TempData["Error"] = $"Error loading skills editor: {e.Message}";
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSkills(EditSkillsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            string? username = User.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                EditSkillsViewModel currentModel = await profileService.GetEditSkillsAsync(username);
                model.CurrentSkills = currentModel.CurrentSkills;
            }

            return View(model);
        }

        try
        {
            string? username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning($"EditFilmmaker POST: {UsernameIsNullOrEmptyMessage}");
                return View("NotFound", NotFoundMessage);
            }

            await profileService.UpdateSkillsAsync(username, model);

            TempData["SuccessMessage"] = "Skills updated successfully!";
            return RedirectToAction("FilmmakerProfile");
        }
        catch (Exception e)
        {
            logger.LogError(e, "EditSkills POST exception: {ExMessage}", e.Message);
            ModelState.AddModelError("", $"Error updating skills: {e.Message}");
            return View(model);
        }
    }
}