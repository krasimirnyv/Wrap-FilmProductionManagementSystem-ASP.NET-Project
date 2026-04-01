namespace Wrap.Services.Core;

using Interfaces;
using Utilities.Providers.Interfaces;
using Models.General;
using Wrap.Data.Models;
using Wrap.Data.Repository.Interfaces;

public class HomeService(IHomeRepository homeRepository, IDateTimeProvider dateTimeProvider) : IHomeService
{
    public async Task<DashboardDataDto> GetDashboardDataAsync()
    {
        DateTime now = dateTimeProvider.Now;
        
        int crewCount = await homeRepository.GetCrewCountAsync();
        int castCount = await homeRepository.GetCastCountAsync();

        IReadOnlyCollection<Production> productions = await homeRepository.GetProductionsAsync();

        IReadOnlyCollection<ProductionDashboardDto> productionsDto = productions
            .Select(p => new ProductionDashboardDto
            {
                Title = p.Title,
                Description = p.Description,
                StatusType = p.StatusType,
                UpcomingScenesCount = p.Scenes
                    .SelectMany(s => s.ShootingDayScenes)
                    .Count(sds => sds.ShootingDay.Date > now)
            })
            .ToArray();

        int upcomingScenesTotal = productionsDto.Sum(p => p.UpcomingScenesCount);

        DashboardDataDto dashboardDataDto = new DashboardDataDto
        {
            CrewMembersCount = crewCount,
            CastMembersCount = castCount,
            UpcomingScenesTotal = upcomingScenesTotal,
            Productions = productionsDto
        };
        
        return dashboardDataDto;
    }
}