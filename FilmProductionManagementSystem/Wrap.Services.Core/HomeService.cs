namespace Wrap.Services.Core;

using Microsoft.EntityFrameworkCore;

using Data;

using ViewModels.General;
using Interface;

using GCommon.Enums;

using static ViewModels.General.Helper.ProductionStatusAbstraction;

public class HomeService(FilmProductionDbContext context) : IHomeService
{
    public async Task<GeneralPageViewModel> GetGeneralInformation()
    {
        IReadOnlyDictionary<string, IReadOnlyCollection<ProductionStatusType>> statusMap = GetStatusTypeByAbstraction();
        DateTime now = DateTime.Now;

        var productionRaw = await context
            .Productions
            .AsNoTracking()
            .Select(p => new
            {
                p.Title,
                p.Description,
                p.StatusType,
                UpcomingScenesCount = p.Scenes
                    .SelectMany(s => s.ShootingDayScenes)
                    .Count(sds => sds.ShootingDay.Date > now)
            })
            .ToArrayAsync();

        int crewCount = await context
            .CrewMembers
            .AsNoTracking()
            .CountAsync();
        
        int castCount = await context
            .CastMembers
            .AsNoTracking()
            .CountAsync();

        GeneralPageViewModel general = new GeneralPageViewModel()
        {
            CrewMembers = crewCount,
            CastMembers = castCount,
            UpcomingScenes = productionRaw.Sum(p => p.UpcomingScenesCount),
            Productions = productionRaw
                .Select(p => new ProductionViewModel()
                {
                    Title = p.Title,
                    Description = p.Description,
                    Status = p.StatusType.ToString(),
                    AbstractStatus = statusMap.First(kvp => kvp.Value.Contains(p.StatusType)).Key
                })
                .ToArray()
        };

        return general;
    }
}