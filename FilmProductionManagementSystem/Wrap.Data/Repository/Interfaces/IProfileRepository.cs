namespace Wrap.Data.Repository.Interfaces;

using Microsoft.EntityFrameworkCore.Storage;

using Models;
using Models.MappingEntities;
using Dtos.Crew;
using Dtos.Cast;

public interface IProfileRepository
{
    Task<Crew?> GetCrewByUsernameAsNoTrackingAsync(string username);
    
    Task<Cast?> GetCastByUsernameAsNoTrackingAsync(string username);
    
    Task<Crew?> GetCrewWithAllDataIncludedByUsernameAsNoTrackingAsync(string username);
    
    Task<Cast?> GetCastWithAllDataIncludedByUsernameAsNoTrackingAsync(string username);
    
    Task<Crew?> GetCrewByUsernameAsync(string username);
    
    Task<Cast?> GetCastByUsernameAsync(string username);

    Task<IReadOnlyCollection<CrewSkill>> GetCrewSkillsAsync(Guid crewId);

    Task<IReadOnlyCollection<ProductionCrew>> GetCrewProductionsAsync(Guid crewId);
    
    Task<IReadOnlyCollection<SceneCrew>> GetCrewScenesAsync(Guid crewId);

    Task<IReadOnlyCollection<ProductionCast>> GetCastProductionsAsync(Guid castId);
    
    Task<IReadOnlyCollection<SceneCast>> GetCastScenesAsync(Guid castId);

    Task<IReadOnlyCollection<CrewSkill>> GetCrewSkillsForUpdateAsync(Guid crewId);

    Task AddCrewSkillsAsync(IEnumerable<CrewSkill> skillsToAdd);
    
    Task RemoveCrewSkillsAsync(IEnumerable<CrewSkill> skillsToRemove);
    
    Task<bool> DeleteCrewProfileAsync(Guid crewId);

    Task<bool> DeleteCastProfileAsync(Guid castId);
    
    Task<CrewPersonalDataDto[]?> DownloadCrewDataAsync(string username);
    
    Task<CastPersonalDataDto[]?> DownloadCastDataAsync(string username);

    Task<int> SaveAllChangesAsync();

    Task<IDbContextTransaction> BeginTransactionAsync();
    
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    
    Task RollbackTransactionAsync(IDbContextTransaction transaction);
}