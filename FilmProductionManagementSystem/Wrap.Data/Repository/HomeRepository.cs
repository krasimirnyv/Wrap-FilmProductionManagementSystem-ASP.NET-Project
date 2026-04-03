namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;

using Interfaces;
using Models;
using GCommon.Enums;

public class HomeRepository(FilmProductionDbContext dbContext)
    : BaseRepository(dbContext), IHomeRepository
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
}