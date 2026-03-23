namespace Wrap.ViewModels.Production;

public class DeleteProductionViewModel
{
    public string Id { get; set; } = null!;
    
    public string Title { get; set; } = null!;
    
    public string? Thumbnail { get; set; }
    
    public string? Description { get; set; }
    
    public string StatusType { get; set; } = null!;
    
    public string Budget { get; set; } = null!;
    
    public int CrewMembersCount { get; set; }
    
    public int CastMembersCount { get; set; }
    
    public int ScenesCount { get; set; }
}
