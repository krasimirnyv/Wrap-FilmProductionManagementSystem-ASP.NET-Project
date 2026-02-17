namespace Wrap.ViewModels.Production;

using HelperViewModels;

public class DetailsProductionViewModel
{
    public string Id { get; set; } = null!;
    
    public string Thumbnail { get; set; } = null!;
    
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }

    public string Budget { get; set; } = null!;

    public string StatusType { get; set; } = null!;
    
    public string StatusAbstractClass { get; set; } = "status-default";

    public string StatusStartDate { get; set; } = null!;

    public string? StatusEndDate { get; set; }
    
    public string? ScriptId { get; set; }
    
    public IReadOnlyCollection<ProductionCrewViewModel> ProductionCrewMembers { get; set; }
        = new List<ProductionCrewViewModel>();

    public IReadOnlyCollection<ProductionCastViewModel> ProductionCastMembers { get; set; }
        = new List<ProductionCastViewModel>();

    public IReadOnlyCollection<ProductionSceneViewModel> Scenes { get; set; }
        = new List<ProductionSceneViewModel>();
    
    public IReadOnlyCollection<ProductionAssetViewModel> ProductionAssets { get; set; }
        = new List<ProductionAssetViewModel>();
    
    public IReadOnlyCollection<ProductionShootingDayViewModel> ShootingDays { get; set; }
        = new List<ProductionShootingDayViewModel>();
}