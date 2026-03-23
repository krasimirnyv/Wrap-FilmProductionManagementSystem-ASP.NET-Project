namespace Wrap.Services.Core.Interfaces;

using Models.NavBar;

public interface INavBarService
{
    /// <summary>
    /// Logic for getting the user data to be displayed in the navigation bar,
    /// such as username, profile image path and role (Crew or Cast).
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>NavBarUserDto</returns>
    Task<NavBarUserDto?> GetNavBarUserAsync(Guid userId);
}