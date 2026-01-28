namespace FilmProductionManagementSystem.Web.Models.MappingEntities;

using System.ComponentModel.DataAnnotations.Schema;

public class SceneCrew
{
    [ForeignKey(nameof(Scene))]
    public Guid SceneId { get; set; }

    public virtual Scene Scene { get; set; } = null!;

    [ForeignKey(nameof(CrewMember))]
    public Guid CrewMemberId { get; set; }

    public virtual Crew CrewMember { get; set; } = null!;
}