namespace Wrap.Data.Repository.Interfaces;

using Models;
using Microsoft.EntityFrameworkCore.Storage;

public interface ILoginRegisterRepository
{
    Task<bool> CrewExistsByUserIdAsync(Guid userId);
    
    Task<bool> CastExistsByUserIdAsync(Guid userId);
    
    Task CreateCrewAsync(Crew crew);
    
    Task CreateCastAsync(Cast cast);

    Task AddCrewSkillsAsync(Guid crewId, IReadOnlyCollection<int> skills);
    
    Task<IDbContextTransaction> BeginTransactionAsync();

    Task CommitTransactionAsync(IDbContextTransaction transaction);
    
    Task RollbackTransactionAsync(IDbContextTransaction transaction);

    Task<int> SaveAllChangesAsync();
}