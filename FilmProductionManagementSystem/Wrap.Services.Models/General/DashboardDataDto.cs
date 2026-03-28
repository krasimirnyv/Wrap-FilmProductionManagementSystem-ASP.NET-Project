namespace Wrap.Services.Models.General;

public class DashboardDataDto
{
    public int CrewMembersCount { get; set; }
    
    public int CastMembersCount { get; set; }
    
    public int UpcomingScenesTotal { get; set; }

    public IReadOnlyCollection<ProductionDashboardDto> Productions { get; set; }
        = new HashSet<ProductionDashboardDto>();
}