namespace Wrap.Data.Repository.Interfaces;

using Models;
using Models.Infrastructure;

public interface IHomeRepository
{
    Task<int> GetCrewCountAsync();
    
    Task<int> GetCastCountAsync();
    
    Task<IReadOnlyCollection<Production>> GetProductionsAsync();

    Task<ApplicationUser?> GetApplicationUserDataAsync(Guid applicationUserId);

    Task<Crew?> GetCrewByUserIdAsync(Guid applicationUserId);
    
    Task<bool> IsUserOwnsProductionsAsync(Guid userId);
}