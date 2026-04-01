namespace FilmProductionManagementSystem.Web.ViewComponents;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Wrap.Data.Models.Infrastructure;
using Wrap.Web.ViewModels.NavBar;
using Wrap.Services.Core.Interfaces;
using Wrap.Services.Models.NavBar;

using static Wrap.GCommon.OutputMessages.NavBar;

public class NavBarUserComponent(UserManager<ApplicationUser> userManager,
                                INavBarService navBarService,
                                ILogger<NavBarUserComponent> logger) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        string? userId = userManager.GetUserId(HttpContext.User);
        if (string.IsNullOrEmpty(userId))
            return View("BadRequestNavBar", UserIsNull);
        
        bool isValidGuid = Guid.TryParse(userId, out Guid userGuid);
        if (!isValidGuid)
            return View("BadRequestNavBar", UserIsNull);

        try
        {
            NavBarUserDto? dto = await navBarService.GetNavBarUserAsync(userGuid);

            if (dto is null)
                return View("BadRequestNavBar", ModelIsNull);

            NavBarUserViewModel viewModel = MapToNavBarUserViewModelFromDto(dto);

            return View(viewModel);
        }
        catch (ArgumentNullException ane)
        {
            logger.LogError(string.Format(UserNotFoundMessage, userId) + ane.Message);
            return View("BadRequestNavBar", UserIsNull + ane.Message);
        }
        catch (Exception e)
        {
            logger.LogError(NavBarFailure + e.Message);
            return View("BadRequestNavBar", NavBarFailure + e.Message);
        }
    }

    private static NavBarUserViewModel MapToNavBarUserViewModelFromDto(NavBarUserDto dto)
    {
        NavBarUserViewModel viewModel = new NavBarUserViewModel
        {
            UserName = dto.UserName,
            ProfileImagePath = dto.ProfileImagePath,
            Role = dto.Role,
        };
        
        return viewModel;
    }
}