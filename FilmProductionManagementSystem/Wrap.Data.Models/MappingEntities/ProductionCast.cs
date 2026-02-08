namespace Wrap.Data.Models.MappingEntities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Mapping Entity - една продукция може да има много актьори и един актьор може да е в много продукции
/// </summary>
public class ProductionCast
{
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }

    public virtual Production Production { get; set; } = null!;

    [ForeignKey(nameof(CastMember))]
    public Guid CastMemberId { get; set; }

    public virtual Cast CastMember { get; set; } = null!;
}