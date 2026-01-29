namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using static Common.EntityConstants.Script;
using static Common.DataValidation;

public class Script
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Unicode]
    [MaxLength(TitleMaxLength)]
    public string Title { get; set; } = null!;

    [Unicode]
    [Column(TypeName = TextTypeFormat)]
    public string? Content { get; set; }
    
    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime LastEditedAt { get; set; }
    
    [Required]
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }  
    
    public virtual Production Production { get; set; } = null!;
}
