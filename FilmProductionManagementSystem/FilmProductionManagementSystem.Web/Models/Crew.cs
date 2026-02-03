namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Enums;
using Infrastructure;
using MappingEntities;

using static Common.EntityConstants.Crew;
using static Common.DataValidation;

public class Crew
{
    [Key]
    public Guid Id { get; set; }
    
    [Unicode]
    [MaxLength(ProfileImagePathMaxLength)]
    public string? ProfileImagePath { get; set; }
    // Example: "/images/crew/john-doe.jpg"

    [Required]
    [Unicode]
    [MaxLength(FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [Unicode]
    [MaxLength(LastNameMaxLength)]
    public string LastName { get; set; } = null!;

    [Unicode(false)]
    [MaxLength(NicknameMaxLength)]
    public string? Nickname { get; set; }
    
    [Required]
    public CrewRoleType RoleType { get; set; }

    [Unicode]
    [MaxLength(BiographyMaxLength)]
    public string? Biography { get; set; }
    
    [Required]
    public PaymentType PaymentType { get; set; }

    [Column(TypeName = DecimalTypeFormat)]
    public decimal PaymentAmount { get; set; }
    
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
        = new List<CrewSkill>();
}