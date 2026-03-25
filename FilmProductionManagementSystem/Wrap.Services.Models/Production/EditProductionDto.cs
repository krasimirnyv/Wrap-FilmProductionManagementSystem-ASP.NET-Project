namespace Wrap.Services.Models.Production;

public class EditProductionDto : CreateProductionDto
{
    public string CurrentThumbnail { get; set; } = null!;
}