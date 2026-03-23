namespace FilmProductionManagementSystem.Web.ViewComponents;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Wrap.Data.Models.Infrastructure;
using Wrap.ViewModels.NavBar;
using Wrap.Services.Core.Interfaces;
using Wrap.Services.Models.NavBar;

using static Wrap.GCommon.OutputMessages.NavBar;

public class NavBarUserComponent(UserManager<ApplicationUser> userManager,
                                INavBarService navBarService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        string? userId = userManager.GetUserId(HttpContext.User);
        if (string.IsNullOrEmpty(userId))
            return View("BadRequestNavBar", UserIsNull);
        
        bool isValidGuid = Guid.TryParse(userId, out Guid userGuid);
        if (!isValidGuid)
            return View("BadRequestNavBar", UserIsNull);
        
        NavBarUserDto? dto = await navBarService.GetNavBarUserAsync(userGuid);
        if (dto is null)
            return View("BadRequestNavBar", ModelIsNull);

        NavBarUserViewModel viewModel = new NavBarUserViewModel
        {
            UserName = dto.UserName,
            ProfileImagePath = dto.ProfileImagePath,
            Role = dto.Role,
        };
        
        return View(viewModel);
    }
}