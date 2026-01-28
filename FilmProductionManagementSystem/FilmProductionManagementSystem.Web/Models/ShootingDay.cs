namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static Common.EntityConstants.ShootingDay;
using static Common.DataValidation;

public class ShootingDay
{
    [Key]
    public Guid Id { get; set; }

    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime Date { get; set; }

    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }
    
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }

    public virtual Production Production { get; set; } = null!;

    public virtual ICollection<ShootingDayScene> ShootingDayScenes { get; set; }
        = new HashSet<ShootingDayScene>();
}
