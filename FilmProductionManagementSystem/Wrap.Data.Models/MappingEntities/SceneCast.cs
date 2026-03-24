namespace Wrap.Data.Models.MappingEntities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using static GCommon.EntityConstants.Cast;

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
    
    /// <summary>
    /// Ролята на актьора няма общо с ролите на снимачния екип
    /// </summary>
    [Unicode]
    [MaxLength(RoleMaxLength)]
    public string? Role { get; set; }
    // Example: "Detective Ivanov"
}