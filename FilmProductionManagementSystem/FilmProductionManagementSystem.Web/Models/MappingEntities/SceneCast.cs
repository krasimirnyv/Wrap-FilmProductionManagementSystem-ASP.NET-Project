namespace FilmProductionManagementSystem.Web.Models.MappingEntities;

using System.ComponentModel.DataAnnotations.Schema;

public class SceneCast
{
    [ForeignKey(nameof(Scene))]
    public Guid SceneId { get; set; }

    public virtual Scene Scene { get; set; } = null!;

    [ForeignKey(nameof(CastMember))]
    public Guid CastMemberId { get; set; }

    public virtual Cast CastMember { get; set; } = null!;
}