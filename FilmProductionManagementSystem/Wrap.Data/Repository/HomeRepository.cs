namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;

using Interfaces;
using Wrap.Services.Models.Home;

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

    public async Task<IReadOnlyCollection<ProductionDashboardDto>> GetProductionSummaryAsync(DateTime now)
    {
        ProductionDashboardDto[] data = await Context!
            .Productions
            .AsNoTracking()
            .Select(p => new  ProductionDashboardDto
            {
                Title = p.Title,
                Description = p.Description,
                StatusType = p.StatusType,
                UpcomingScenesCount = p.Scenes
                    .SelectMany(s => s.ShootingDayScenes)
                    .Count(sds => sds.ShootingDay.Date > now)
            })
            .ToArrayAsync();
        
        return data;
    }
}