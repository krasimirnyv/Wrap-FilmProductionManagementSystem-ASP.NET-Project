namespace Wrap.Data.Repository.Interfaces;

using Microsoft.EntityFrameworkCore.Storage;

using Models;
using GCommon.Enums;

public interface IProfileRepository
{
    Task<Crew?> GetCrewByUsernameAsync(string username);
    
    Task<Cast?> GetCastByUsernameAsync(string username);

    Task<IReadOnlyCollection<CrewSkill>> GetCrewSkillsAsync(Guid crewId);

    Task<IReadOnlyCollection<(Guid ProductionId, string Title, string? Description, string Status, CrewRoleType RoleType)>> GetCrewProductionsAsync(Guid crewId);
    
    Task<IReadOnlyCollection<(Guid SceneId, string SceneName, string ProductionTitle, CrewRoleType RoleType)>> GetCrewScenesAsync(Guid crewId);

    Task<IReadOnlyCollection<(Guid ProductionId, string Title, string? Description, string Status, string? CharacterName)>> GetCastProductionsAsync(Guid castId);
    
    Task<IReadOnlyCollection<(Guid SceneId, string SceneName, string ProductionTitle, string? CharacterName)>> GetCastScenesAsync(Guid castId);

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