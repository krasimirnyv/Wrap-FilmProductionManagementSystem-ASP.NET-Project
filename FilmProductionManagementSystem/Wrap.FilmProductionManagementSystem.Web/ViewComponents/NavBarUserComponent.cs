namespace FilmProductionManagementSystem.Web.ViewComponents;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Wrap.Data.Models.Infrastructure;
using Wrap.ViewModels.NavBar;
using Wrap.Services.Core.Interface;

using static Wrap.GCommon.OutputMessages.NavBar;

public class NavBarUserComponent(UserManager<ApplicationUser> userManager,
                                INavBarService navBarService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        ApplicationUser? user = await userManager.GetUserAsync(HttpContext.User);
        if (user is null)
            return View("BadRequestNavBar", UserIsNull);
        
        NavBarUserViewModel? model = await navBarService.GetNavBarUserAsync(user.Id);
        if (model is null)
            return View("BadRequestNavBar", ModelIsNull);
        
        return View(model);
    }
}