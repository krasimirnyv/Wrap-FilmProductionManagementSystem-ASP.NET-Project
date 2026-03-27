namespace Wrap.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using GCommon.Enums;

using static GCommon.EntityConstants.Script;
using static GCommon.DataFormat;

/// <summary>
/// Entity за сценарий, който принадлежи към дадена продукция
/// </summary>
public class Script
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Unicode]
    [MaxLength(TitleMaxLength)]
    public string Title { get; set; } = null!;
    
    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime LastEditedAt { get; set; }
    
    [Required]
    public ScriptStageType StageType { get; set; }
    
    public ScriptRevisionType?  RevisionType { get; set; }
    
    [Required]
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }  
    
    public virtual Production Production { get; set; } = null!;
    
    public virtual ICollection<ScriptBlock> ScriptBlocks { get; set; }
        = new HashSet<ScriptBlock>();
}
