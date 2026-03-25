namespace Wrap.Services.Models.Production;

using GCommon.Enums;

public class DeleteProductionDto
{
    public Guid Id { get; set; }
    
    public string Title { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public ProductionStatusType StatusType { get; set; }
    
    public decimal Budget { get; set; }
    
    public int CrewMembersCount { get; set; }
    
    public int CastMembersCount { get; set; }
    
    public int ScenesCount { get; set; }
}