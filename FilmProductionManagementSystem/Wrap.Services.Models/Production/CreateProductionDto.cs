namespace Wrap.Services.Models.Production;

using Microsoft.AspNetCore.Http;

using GCommon.Enums;

public class CreateProductionDto
{
    public IFormFile? ThumbnailImage { get; set; }
    
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public decimal Budget { get; set; }

    public ProductionStatusType StatusType { get; set; }
    
    public DateTime StatusStartDate { get; set; }

    public DateTime? StatusEndDate { get; set; }

    public Guid CreatorId { get; set; }
}