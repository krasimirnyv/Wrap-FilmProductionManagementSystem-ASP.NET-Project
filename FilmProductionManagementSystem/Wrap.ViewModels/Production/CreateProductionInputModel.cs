namespace Wrap.ViewModels.Production;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

using GCommon.Enums;

using static GCommon.EntityConstants.Production;

public class CreateProductionInputModel
{
    public IFormFile? Thumbnail { get; set; }
    
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
    public string Title { get; set; } = null!;
    
    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }
    
    [Range(BudgetMinValue, double.MaxValue)]
    public decimal Budget { get; set; }

    [Required]
    public ProductionStatusType StatusType { get; set; }
    
    [Required]
    public DateTime StatusStartDate { get; set; }

    public DateTime? StatusEndDate { get; set; }
}