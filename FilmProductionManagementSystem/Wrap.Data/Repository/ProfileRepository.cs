namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Interfaces;
using Models;
using GCommon.Enums;

public class ProfileRepository(FilmProductionDbContext dbContext)
    : BaseRepository(dbContext), IProfileRepository
{
    
    public async Task<Crew?> GetCrewByUsernameAsync(string username)
    {
        Crew? crew = await Context!
            .CrewMembers
            .Include(c => c.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return crew;
    }

    public async Task<Cast?> GetCastByUsernameAsync(string username)
    {
        Cast? cast = await Context!
            .CastMembers
            .Include(c => c.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return cast;
    }

    public async Task<IReadOnlyCollection<CrewSkill>> GetCrewSkillsAsync(Guid crewId)
    {
        IReadOnlyCollection<CrewSkill> skills = await Context!
            .CrewSkills
            .AsNoTracking()
            .Where(cs => cs.CrewMemberId == crewId)
            .ToArrayAsync();
        
        return skills;
    }

    public async Task<IReadOnlyCollection<(Guid ProductionId, string Title, string? Description, string Status, CrewRoleType RoleType)>> GetCrewProductionsAsync(Guid crewId)
    {
        IReadOnlyCollection<(Guid ProductionId, string Title, string? Description, string Status, CrewRoleType RoleType)> crewProductions =
            await Context!
                .ProductionsCrewMembers
                .Include(pc => pc.Production)
                .AsNoTracking()
                .Where(pc => pc.CrewMemberId == crewId)
                .Select(pc => new ValueTuple<Guid, string, string?, string, CrewRoleType>(
                    pc.ProductionId,
                    pc.Production.Title,
                    pc.Production.Description,
                    pc.Production.StatusType.ToString(),
                    pc.RoleType
                ))
                .ToArrayAsync();
        
        return crewProductions;
    }

    public async Task<IReadOnlyCollection<(Guid SceneId, string SceneName, string ProductionTitle, CrewRoleType RoleType)>> GetCrewScenesAsync(Guid crewId)
    {
        IReadOnlyCollection<(Guid SceneId, string SceneName, string ProductionTitle, CrewRoleType RoleType)> crewScenes = await Context!
            .ScenesCrewMembers
            .Include(sc => sc.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(sc => sc.CrewMemberId == crewId)
            .Select(sc => new ValueTuple<Guid, string, string, CrewRoleType>(
                sc.SceneId,
                sc.Scene.SceneName,
                sc.Scene.Production.Title,
                sc.RoleType
            ))
            .ToArrayAsync();
        
        return crewScenes;
    }

    public async Task<IReadOnlyCollection<(Guid ProductionId, string Title, string? Description, string Status, string? CharacterName)>> GetCastProductionsAsync(Guid castId)
    {
        IReadOnlyCollection<(Guid ProductionId, string Title, string? Description, string Status, string? CharacterName)> castProductions = await Context!
                .ProductionsCastMembers
                .Include(pc => pc.Production)
                .AsNoTracking()
                .Where(pc => pc.CastMemberId == castId)
                .Select(pc => new ValueTuple<Guid, string, string?, string, string?>(
                    pc.ProductionId,
                    pc.Production.Title,
                    pc.Production.Description,
                    pc.Production.StatusType.ToString(),
                    pc.Role
                ))
                .ToArrayAsync();

        return castProductions;
    }

    public async Task<IReadOnlyCollection<(Guid SceneId, string SceneName, string ProductionTitle, string? CharacterName)>> GetCastScenesAsync(Guid castId)
    {
        IReadOnlyCollection<(Guid SceneId, string SceneName, string ProductionTitle, string? CharacterName)> castScenes = await Context!
            .ScenesCastMembers
            .Include(sc => sc.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(sc => sc.CastMemberId == castId)
            .Select(sc => new ValueTuple<Guid, string, string, string?>(
                sc.SceneId,
                sc.Scene.SceneName,
                sc.Scene.Production.Title,
                sc.Role
            ))
            .ToArrayAsync();
        
        return castScenes;
    }

    public async Task<Crew?> GetCrewForUpdateAsync(string username)
    {
        Crew? crew = await Context!
            .CrewMembers
            .Include(c => c.User)
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return crew;
    }

    public async Task<Cast?> GetCastForUpdateAsync(string username)
    {
        Cast? cast = await Context!
            .CastMembers
            .Include(c => c.User)
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return cast;
    }

    public async Task<IReadOnlyCollection<CrewSkill>> GetCrewSkillsForUpdateAsync(Guid crewId)
    {
        IReadOnlyCollection<CrewSkill> skills = await Context!
            .CrewSkills
            .Where(c => c.CrewMemberId == crewId)
            .ToArrayAsync();
        
        return skills;
    }

    public async Task AddCrewSkillsAsync(IEnumerable<CrewSkill> skillsToAdd)
    {
        await Context!.AddRangeAsync(skillsToAdd);
    }

    public Task RemoveCrewSkillsAsync(IEnumerable<CrewSkill> skillsToRemove)
    {
        Context!.RemoveRange(skillsToRemove);
        return Task.CompletedTask;
    }

    public async Task<int> SaveAllChangesAsync()
    {
        int affectedRows = await SaveChangesAsync();
        return affectedRows;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        IDbContextTransaction transaction = await Context!.Database.BeginTransactionAsync();
        return transaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        await transaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
    {
        await transaction.RollbackAsync();
    }
}