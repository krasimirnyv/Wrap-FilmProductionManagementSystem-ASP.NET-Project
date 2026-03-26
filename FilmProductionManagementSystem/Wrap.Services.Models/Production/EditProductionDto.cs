namespace Wrap.Services.Models.Production;

public class EditProductionDto : CreateProductionDto
{
    public Guid ProductionId { get; set; }
    
    public string? CurrentThumbnailPath { get; set; }
}