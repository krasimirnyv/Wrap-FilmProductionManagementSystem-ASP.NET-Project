namespace Wrap.Services.Core.Interface;

using ViewModels.NavBar;

public interface INavBarService
{
    Task<NavBarUserViewModel?> GetNavBarUserAsync(string userId);
}