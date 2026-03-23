namespace Wrap.Services.Models.Home;

public class DashboardDataDto
{
    public int CrewMembersCount { get; set; }
    
    public int CastMembersCount { get; set; }
    
    public int UpcomingScenesTotal { get; set; }

    public IReadOnlyCollection<ProductionDashboardDto> Productions { get; set; } = [];
}