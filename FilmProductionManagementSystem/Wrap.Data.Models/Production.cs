namespace Wrap.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using GCommon.Enums;
using MappingEntities;

using static GCommon.EntityConstants.Production;
using static GCommon.DataFormat;

/// <summary>
/// Това Entity представлява филмова продукцията, която съдържа 1 сценарий, колекция от екип и актьори, от сцени, инструменти и снимачни дни
/// </summary>
public class Production
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Unicode]
    [MaxLength(TitleMaxLength)]
    public string Title { get; set; } = null!;

    [Unicode]
    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }

    [Column(TypeName = DecimalTypeFormat)]
    public decimal Budget { get; set; }

    [Unicode]
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

    public virtual ICollection<ProductionCrew> ProductionCrewMembers { get; set; }
        = new List<ProductionCrew>();
    
    public virtual ICollection<ProductionCast> ProductionCastMembers { get; set; }
        = new List<ProductionCast>();
    
    public virtual ICollection<Scene> Scenes { get; set; }
        = new HashSet<Scene>();

    public virtual ICollection<ProductionAsset> ProductionAssets { get; set; }
        = new List<ProductionAsset>();

    public virtual ICollection<ShootingDay> ShootingDays { get; set; }
        = new List<ShootingDay>();
    
}