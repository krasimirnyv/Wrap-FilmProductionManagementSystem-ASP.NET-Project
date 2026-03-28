namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Interfaces;
using Models;
using Models.MappingEntities;

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

    public async Task<IReadOnlyCollection<ProductionCrew>> GetCrewProductionsAsync(Guid crewId)
    {
        IReadOnlyCollection<ProductionCrew> crewProductions = await Context!
            .ProductionsCrewMembers
            .Include(c => c.Production)
            .AsNoTracking()
            .Where(pc => pc.CrewMemberId == crewId)
            .ToArrayAsync();
        
        return crewProductions;
    }

    public async Task<IReadOnlyCollection<SceneCrew>> GetCrewScenesAsync(Guid crewId)
    {
        IReadOnlyCollection<SceneCrew> crewScenes = await Context!
            .ScenesCrewMembers
            .Include(sc => sc.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(sc => sc.CrewMemberId == crewId)
            .ToArrayAsync();
        
        return crewScenes;
    }

    public async Task<IReadOnlyCollection<ProductionCast>> GetCastProductionsAsync(Guid castId)
    {
        IReadOnlyCollection<ProductionCast> castProductions = await Context!
                .ProductionsCastMembers
                .Include(pc => pc.Production)
                .AsNoTracking()
                .Where(pc => pc.CastMemberId == castId)
                .ToArrayAsync();

        return castProductions;
    }

    public async Task<IReadOnlyCollection<SceneCast>> GetCastScenesAsync(Guid castId)
    {
        IReadOnlyCollection<SceneCast> castScenes = await Context!
            .ScenesCastMembers
            .Include(sc => sc.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(sc => sc.CastMemberId == castId)
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