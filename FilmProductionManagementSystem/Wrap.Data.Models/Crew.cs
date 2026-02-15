namespace Wrap.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using GCommon.Enums;
using Infrastructure;
using MappingEntities;

using static GCommon.EntityConstants.Crew;
using static GCommon.DataValidation;

/// <summary>
/// Entity представящо човек от снимачен екип
/// </summary>
public class Crew
{
    [Key]
    public Guid Id { get; set; }

    [Unicode]
    [Required]
    [MaxLength(ProfileImagePathMaxLength)]
    public string ProfileImagePath { get; set; } = null!;
    // Example: "/images/crew/john-doe.jpg"

    [Required]
    [Unicode]
    [MaxLength(FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [Unicode]
    [MaxLength(LastNameMaxLength)]
    public string LastName { get; set; } = null!;

    [Unicode]
    [MaxLength(NicknameMaxLength)]
    public string? Nickname { get; set; }

    /// <summary>
    /// Роляата в дадена продукция на човек от екипа ще се имплементира по-нататък
    /// </summary>
    // [Required]
    // public CrewRoleType RoleType { get; set; }
    
    [Unicode]
    [MaxLength(BiographyMaxLength)]
    public string? Biography { get; set; }
    
    /// <summary>
    /// Типът на заплащане и паричната сума ще се импелементират по-нататък
    /// </summary>
    // public PaymentType? PaymentType { get; set; }
    //
    // [Column(TypeName = DecimalTypeFormat)]
    // public decimal? PaymentAmount { get; set; }
    
    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }
    
    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
    
    public virtual ICollection<ProductionCrew> CrewMemberProductions { get; set; }
        = new List<ProductionCrew>();

    public virtual ICollection<SceneCrew> CrewMemberScenes { get; set; }
        = new List<SceneCrew>();
    
    public virtual ICollection<CrewSkill> Skills { get; set; }
        = new HashSet<CrewSkill>();
}