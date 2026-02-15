namespace Wrap.ViewModels.General;

public class GeneralPageViewModel
{
    public int CrewMembers { get; set; }
    
    public int CastMembers { get; set; }
    
    public int UpcomingScenes { get; set; }
    
    public IEnumerable<ProductionViewModel> Productions { get; set; } = null!;
}