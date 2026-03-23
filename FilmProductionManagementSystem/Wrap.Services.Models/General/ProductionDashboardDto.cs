namespace Wrap.Services.Models.General;

using GCommon.Enums;

public class ProductionDashboardDto
{
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public ProductionStatusType StatusType { get; set; }
    
    public int UpcomingScenesCount { get; set; }
}