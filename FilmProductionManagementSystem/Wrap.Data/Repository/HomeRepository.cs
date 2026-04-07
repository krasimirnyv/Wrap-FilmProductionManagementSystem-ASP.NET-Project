namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;

using Interfaces;
using Models;
using Models.Infrastructure;
using GCommon.Enums;

public class HomeRepository(FilmProductionDbContext dbContext) : BaseRepository(dbContext), IHomeRepository
{
    public async Task<int> GetCrewCountAsync()
    {
        int crewCount = await Context!
            .CrewMembers
            .AsNoTracking()
            .CountAsync();
        
        return crewCount;
    }

    public async Task<int> GetCastCountAsync()
    {
        int castCount = await Context!
            .CastMembers
            .AsNoTracking()
            .CountAsync();
        
        return castCount;
    }

    public async Task<IReadOnlyCollection<Production>> GetProductionsAsync()
    {
        IReadOnlyCollection<Production> productions = await Context!
            .Productions
            .AsNoTracking()
            .Where(p =>
                p.StatusType == ProductionStatusType.Production ||
                p.StatusType == ProductionStatusType.OnHold ||
                p.StatusType == ProductionStatusType.Reshoots)
            .ToArrayAsync();
        
        return productions;
    }

    public async Task<ApplicationUser?> GetApplicationUserDataAsync(Guid applicationUserId)
    {
        ApplicationUser? user = await Context!
            .Users
            .AsNoTracking()
            .SingleOrDefaultAsync(au => au.Id == applicationUserId);
        
        return user;
    }

    public async Task<Crew?> GetCrewByUserIdAsync(Guid applicationUserId)
    {
        Crew? crew = await Context!
            .CrewMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.UserId == applicationUserId);
        
        return crew;
    }

    public async Task<bool> IsUserOwnsProductionsAsync(Guid userId)
    {
        bool hasProductions = await Context!
            .Productions
            .AsNoTracking()
            .AnyAsync(p => p.CreatedByUserId == userId);
        
        return hasProductions;
    }
}