namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Enums;

using static Common.EntityConstants.Production;
using static Common.DataValidation;

public class Production
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(TitleMaxLength)]
    public string Title { get; set; } = null!;

    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }

    [Column(TypeName = DecimalTypeFormat)]
    public decimal Budget { get; set; }

    [MaxLength(ThumbnailMaxLength)]
    public string? Thumbnail { get; set; }
    // Example with image path: "/images/productions/production-1.png"

    [Required]
    public ProductionStatusType StatusType { get; set; }

    [Required]
    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime StatusStartDate { get; set; }

    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime? StatusEndDate { get; set; }

    public virtual Script? Script { get; set; }

    public virtual ICollection<Scene> Scenes { get; set; }
        = new HashSet<Scene>();

    public virtual ICollection<ProductionAsset> ProductionAssets { get; set; }
        = new List<ProductionAsset>();

    public virtual ICollection<ShootingDay> ShootingDays { get; set; }
        = new List<ShootingDay>();
}