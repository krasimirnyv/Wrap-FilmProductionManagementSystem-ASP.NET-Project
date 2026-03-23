namespace Wrap.Data.Repository.Interfaces;

using Wrap.Services.Models.NavBar;

public interface INavBarRepository
{
    Task<NavBarUserDto?> GetNavBarCrewUserAsync(Guid userId);
    
    Task<NavBarUserDto?> GetNavBarCastUserAsync(Guid userId);
}