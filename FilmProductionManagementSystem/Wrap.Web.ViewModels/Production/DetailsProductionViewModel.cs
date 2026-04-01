namespace Wrap.Web.ViewModels.Production;

using NestedViewModels;

using static GCommon.ApplicationConstants;

public class DetailsProductionViewModel
{
    public string Id { get; set; } = null!;
    
    public string Thumbnail { get; set; } = null!;
    
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }

    public string Budget { get; set; } = null!;

    public string StatusType { get; set; } = null!;
    
    public string StatusAbstractClass { get; set; } = DefaultStatus;

    public string StatusStartDate { get; set; } = null!;

    public string? StatusEndDate { get; set; }
    
    public string? ScriptId { get; set; }
    
    public IReadOnlyCollection<ProductionCrewMemberViewModel> ProductionCrewMembers { get; set; }
        = new List<ProductionCrewMemberViewModel>();

    public IReadOnlyCollection<ProductionCastMemberViewModel> ProductionCastMembers { get; set; }
        = new List<ProductionCastMemberViewModel>();

    public IReadOnlyCollection<ProductionSceneViewModel> ProductionScenes { get; set; }
        = new List<ProductionSceneViewModel>();
    
    public IReadOnlyCollection<ProductionAssetViewModel> ProductionAssets { get; set; }
        = new List<ProductionAssetViewModel>();
    
    public IReadOnlyCollection<ProductionShootingDayViewModel> ProductionShootingDays { get; set; }
        = new List<ProductionShootingDayViewModel>();
}