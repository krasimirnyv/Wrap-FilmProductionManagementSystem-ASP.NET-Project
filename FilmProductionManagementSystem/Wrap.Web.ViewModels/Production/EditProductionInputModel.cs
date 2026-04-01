namespace Wrap.Web.ViewModels.Production;

using System.ComponentModel.DataAnnotations;

public class EditProductionInputModel : CreateProductionInputModel
{
    [Required]
    public string ProductionId { get; set; } = null!;
    
    public string? CurrentThumbnailPath { get; set; }
}