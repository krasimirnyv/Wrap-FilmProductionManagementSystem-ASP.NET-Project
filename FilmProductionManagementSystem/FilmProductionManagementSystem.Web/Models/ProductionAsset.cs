namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Enums;

using static Common.EntityConstants.ProductionAsset;
using static Common.DataValidation;

public class ProductionAsset
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public ProductionAssetType AssetType { get; set; }

    [Required]
    [Unicode]
    [MaxLength(TitleMaxLength)]
    public string Title { get; set; } = null!;

    [Unicode]
    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }
    
    [Required]
    [Unicode]
    [MaxLength(FilePathMaxLength)]
    public string FilePath { get; set; } = null!;

    [Unicode(false)]
    [MaxLength(FileTypeMaxLength)]
    public string? FileType { get; set; }

    [Required]
    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime UploadedAt { get; set; }
    
    [Required]
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }

    public virtual Production Production { get; set; } = null!;
}