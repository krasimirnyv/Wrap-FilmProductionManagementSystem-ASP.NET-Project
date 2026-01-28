namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Enums;
using Infrastructure;

using static Common.EntityConstants.Crew;
using static Common.DataValidation;

public class Crew
{
    [Key]
    public Guid Id { get; set; }
    
    [MaxLength(ProfileImagePathMaxLength)]
    public string? ProfileImagePath { get; set; }
    // Example: "/images/crew/john-doe.jpg"

    [Required]
    [MaxLength(FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(LastNameMaxLength)]
    public string LastName { get; set; } = null!;

    [MaxLength(NicknameMaxLength)]
    public string? Nickname { get; set; }
    
    [Required]
    public CrewRoleType RoleType { get; set; }

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
    
    public virtual ICollection<Production> Productions { get; set; }
        = new List<Production>();

    public virtual ICollection<Scene> Scenes { get; set; }
        = new List<Scene>();
}