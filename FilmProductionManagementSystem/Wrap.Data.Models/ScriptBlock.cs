namespace Wrap.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using GCommon.Enums;

using static GCommon.DataValidation;

/// <summary>
/// Това Entity представлява единичен блок от сценария (име на сцена, дейсвие, диалог, транзишън и тн.)
/// Съхранява структурираното съдържание на сценария за правилно запазване и манипулиране
/// </summary>
public class ScriptBlock
{
    [Key]
    public Guid Id { get; set; }
    
    public int OrderIndex { get; set; }
    
    [Required]
    public ScriptBlockType BlockType { get; set; }
    
    [Unicode]
    [Column(TypeName = TextTypeFormat)]
    public string? Content { get; set; }

    /// <summary>
    /// Незадължителни метаданни (име на герой за автоматично довършване, номера на сцени и др.)
    /// Съхранява се като JSON string
    /// </summary>
    [Unicode]
    [Column(TypeName = TextTypeFormat)]
    public string? Metadata { get; set; }

    [Required]
    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime CreatedAt { get; set; }

    [Required]
    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime LastModifiedAt { get; set; }
    
    [Required]
    [ForeignKey(nameof(Script))]
    public Guid ScriptId { get; set; }

    public virtual Script Script { get; set; } = null!;
}
