namespace Wrap.Data.Models.MappingEntities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Mapping Entity - една продукция може да има много хора в снимачения екип и един човек от снимачния екип може да е в много продукции
/// </summary>
public class ProductionCrew
{
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }

    public virtual Production Production { get; set; } = null!;

    [ForeignKey(nameof(CrewMember))]
    public Guid CrewMemberId { get; set; }

    public virtual Crew CrewMember { get; set; } = null!;
}