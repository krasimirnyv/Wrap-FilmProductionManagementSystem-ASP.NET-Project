namespace Wrap.Data.Models.MappingEntities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using static GCommon.EntityConstants.Cast;

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

    /// <summary>
    /// Ролята на актьора няма общо с ролите на снимачния екип
    /// </summary>
    [Required]
    [Unicode]
    [MaxLength(RoleMaxLength)]
    public string Role { get; set; } = null!;
    // Example: "Detective Ivanov"
}