namespace Wrap.Services.Core.Interface;

using ViewModels.NavBar;

public interface INavBarService
{
    /// <summary>
    /// Logic for getting the user data to be displayed in the navigation bar,
    /// such as username, profile image path and role (Crew or Cast).
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>NavBarUserViewModel</returns>
    Task<NavBarUserViewModel?> GetNavBarUserAsync(string userId);
}