namespace Wrap.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using GCommon.Enums;

/// <summary>
/// Представлява едно умение, което човек от снимачния екип притежава
/// </summary>
public class CrewSkill
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public CrewRoleType RoleType { get; set; }
    
    [Required]
    [ForeignKey(nameof(CrewMember))]
    public Guid CrewMemberId { get; set; }
    
    public virtual Crew CrewMember { get; set; } = null!;
}