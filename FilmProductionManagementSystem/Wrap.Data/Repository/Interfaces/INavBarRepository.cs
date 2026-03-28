namespace Wrap.Data.Repository.Interfaces;

using Models;

public interface INavBarRepository
{
    Task<Crew?> GetCrewUserAsync(Guid userId);
    
    Task<Cast?> GetCastUserAsync(Guid userId);
}