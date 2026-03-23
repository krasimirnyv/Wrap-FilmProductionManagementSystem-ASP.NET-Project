namespace Wrap.Services.Core;

using Interfaces;
using Models.General;
using Wrap.Data.Repository.Interfaces;

public class HomeService(IHomeRepository repository) : IHomeService
{
    public async Task<DashboardDataDto> GetDashboardDataAsync()
    {
        DateTime now = DateTime.Now;
        
        int crewCount = await repository.GetCrewCountAsync();
        int castCount = await repository.GetCastCountAsync();

        IReadOnlyCollection<ProductionDashboardDto> productions = await repository.GetProductionSummaryAsync(now);
        
        int upcomingScenesTotal = productions.Sum(p => p.UpcomingScenesCount);

        DashboardDataDto dashboardDataDto = new DashboardDataDto
        {
            CrewMembersCount = crewCount,
            CastMembersCount = castCount,
            UpcomingScenesTotal = upcomingScenesTotal,
            Productions = productions
        };
        
        return dashboardDataDto;
    }
}