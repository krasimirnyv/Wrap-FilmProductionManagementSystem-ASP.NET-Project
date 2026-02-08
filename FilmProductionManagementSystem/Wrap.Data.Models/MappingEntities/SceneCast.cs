namespace Wrap.Data.Models.MappingEntities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// За момента игнораме това mapping entity, то е за план-графикa
/// </summary>
public class SceneCast
{
    [ForeignKey(nameof(Scene))]
    public Guid SceneId { get; set; }

    public virtual Scene Scene { get; set; } = null!;

    [ForeignKey(nameof(CastMember))]
    public Guid CastMemberId { get; set; }

    public virtual Cast CastMember { get; set; } = null!;
}