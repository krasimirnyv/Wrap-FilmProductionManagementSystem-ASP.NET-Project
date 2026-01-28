namespace FilmProductionManagementSystem.Web.Models.MappingEntities;

using System.ComponentModel.DataAnnotations.Schema;

public class ProductionCast
{
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }

    public virtual Production Production { get; set; } = null!;

    [ForeignKey(nameof(CastMember))]
    public Guid CastMemberId { get; set; }

    public virtual Cast CastMember { get; set; } = null!;
}