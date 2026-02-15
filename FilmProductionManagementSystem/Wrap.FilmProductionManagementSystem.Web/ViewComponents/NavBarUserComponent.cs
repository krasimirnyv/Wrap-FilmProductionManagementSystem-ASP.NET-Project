using Wrap.ViewModels.NavBar;

namespace FilmProductionManagementSystem.Web.ViewComponents;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Wrap.Data.Models.Infrastructure;

using Wrap.Services.Core.Interface;

public class NavBarUserComponent(UserManager<ApplicationUser> userManager,
                                INavBarService navBarService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        ApplicationUser? user = await userManager.GetUserAsync(HttpContext.User);
        if (user is null)
            return View("BadRequestNavBar");
        
        NavBarUserViewModel? model = await navBarService.GetNavBarUserAsync(user.Id);
        if (model is null)
            return View("BadRequestNavBar");
        
        return View(model);
    }
}