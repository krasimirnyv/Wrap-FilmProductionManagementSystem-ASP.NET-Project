namespace Wrap.Data.Models.MappingEntities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// За момента игнораме това mapping entity, то е за план-графикa
/// </summary>
public class SceneCrew
{
    [ForeignKey(nameof(Scene))]
    public Guid SceneId { get; set; }

    public virtual Scene Scene { get; set; } = null!;

    [ForeignKey(nameof(CrewMember))]
    public Guid CrewMemberId { get; set; }

    public virtual Crew CrewMember { get; set; } = null!;
}