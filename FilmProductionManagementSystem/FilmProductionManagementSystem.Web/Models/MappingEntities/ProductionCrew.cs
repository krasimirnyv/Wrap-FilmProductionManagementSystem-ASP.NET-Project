namespace FilmProductionManagementSystem.Web.Models.MappingEntities;

using System.ComponentModel.DataAnnotations.Schema;

public class ProductionCrew
{
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }

    public virtual Production Production { get; set; } = null!;

    [ForeignKey(nameof(CrewMember))]
    public Guid CrewMemberId { get; set; }

    public virtual Crew CrewMember { get; set; } = null!;
}