namespace Wrap.Web.ViewModels.General;

public class DashboardViewModel
{
    public int CrewMembersCount { get; set; }
    
    public int CastMembersCount { get; set; }
    
    public int UpcomingScenesTotal { get; set; }
    
    public IEnumerable<ProductionInfoViewModel> Productions { get; set; } = null!;

    public bool IsUserCrew { get; set; } = false;
    
    public bool HasOwnProductions { get; set; }
}