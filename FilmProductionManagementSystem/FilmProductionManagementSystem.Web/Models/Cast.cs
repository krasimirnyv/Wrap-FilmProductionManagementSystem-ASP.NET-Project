namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Enums;
using Infrastructure;
using MappingEntities;

using static Common.EntityConstants.Cast;
using static Common.DataValidation;

public class Cast
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Unicode]
    [MaxLength(ProfileImagePathMaxLength)]
    public string ProfileImagePath { get; set; } = null!;
    // Example: "/images/cast/jane-doe.jpg"

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
    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime BirthDate { get; set; }
    
    [Required]
    public GenderType Gender { get; set; }

    [Unicode]
    [MaxLength(RoleMaxLength)]
    public string? Role { get; set; }
    // Example: "Detective Ivanov"
    
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

    public virtual ICollection<ProductionCast> CastMemberProductions { get; set; }
        = new List<ProductionCast>();

    public virtual ICollection<SceneCast> CastMemberScenes { get; set; }
        = new List<SceneCast>();
    
    [NotMapped]
    public int Age =>
        DateTime.Today.Year - BirthDate.Year -
        (BirthDate.Date > DateTime.Today.AddYears(
            -(DateTime.Today.Year - BirthDate.Year)) ? 1 : 0);
}