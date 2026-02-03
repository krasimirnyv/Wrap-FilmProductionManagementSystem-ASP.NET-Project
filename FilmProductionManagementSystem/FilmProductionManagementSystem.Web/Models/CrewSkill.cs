namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Enums;

public class CrewSkill
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [ForeignKey(nameof(CrewMember))]
    public Guid CrewMemberId { get; set; }
    
    public virtual Crew CrewMember { get; set; } = null!;

    [Required]
    public CrewRoleType RoleType { get; set; }
}