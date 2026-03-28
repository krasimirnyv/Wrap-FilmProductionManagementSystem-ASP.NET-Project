namespace Wrap.Data.Repository.Interfaces;

using Models;

public interface IHomeRepository
{
    Task<int> GetCrewCountAsync();
    
    Task<int> GetCastCountAsync();
    
    Task<IReadOnlyCollection<Production>> GetProductionsAsync();
}