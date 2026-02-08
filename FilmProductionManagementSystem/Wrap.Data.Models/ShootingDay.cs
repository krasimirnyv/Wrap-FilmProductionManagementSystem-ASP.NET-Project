namespace Wrap.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using static GCommon.EntityConstants.ShootingDay;
using static GCommon.DataValidation;

/// <summary>
/// Това entity ще се изпозлва за по-нататък, когато се започне изграждането на план-график възможност в апликацията
/// За момента ще бъде игнориран
/// </summary>
public class ShootingDay
{
    [Key]
    public Guid Id { get; set; }

    [Column(TypeName = DateTimeTypeFormat)]
    public DateTime Date { get; set; }

    [Unicode]
    [MaxLength(NotesMaxLength)]
    public string? Notes { get; set; }
    
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }

    public virtual Production Production { get; set; } = null!;

    public virtual ICollection<ShootingDayScene> ShootingDayScenes { get; set; }
        = new HashSet<ShootingDayScene>();
}
