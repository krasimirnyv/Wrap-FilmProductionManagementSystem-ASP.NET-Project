namespace Wrap.Data.Repository.Interfaces;

using Microsoft.EntityFrameworkCore.Storage;

using Models;
using Models.MappingEntities;

public interface IProfileRepository
{
    Task<Crew?> GetCrewByUsernameAsync(string username);
    
    Task<Cast?> GetCastByUsernameAsync(string username);

    Task<IReadOnlyCollection<CrewSkill>> GetCrewSkillsAsync(Guid crewId);

    Task<IReadOnlyCollection<ProductionCrew>> GetCrewProductionsAsync(Guid crewId);
    
    Task<IReadOnlyCollection<SceneCrew>> GetCrewScenesAsync(Guid crewId);

    Task<IReadOnlyCollection<ProductionCast>> GetCastProductionsAsync(Guid castId);
    
    Task<IReadOnlyCollection<SceneCast>> GetCastScenesAsync(Guid castId);

    Task<Crew?> GetCrewForUpdateAsync(string username);
    
    Task<Cast?> GetCastForUpdateAsync(string username);

    Task<IReadOnlyCollection<CrewSkill>> GetCrewSkillsForUpdateAsync(Guid crewId);

    Task AddCrewSkillsAsync(IEnumerable<CrewSkill> skillsToAdd);
    
    Task RemoveCrewSkillsAsync(IEnumerable<CrewSkill> skillsToRemove);

    Task<int> SaveAllChangesAsync();

    Task<IDbContextTransaction> BeginTransactionAsync();
    
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    
    Task RollbackTransactionAsync(IDbContextTransaction transaction);
}