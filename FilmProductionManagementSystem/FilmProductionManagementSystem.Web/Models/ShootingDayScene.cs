namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ShootingDayScene
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public int Order { get; set; }

    [ForeignKey(nameof(ShootingDay))]
    public Guid ShootingDayId { get; set; }
    
    public virtual ShootingDay ShootingDay { get; set; } = null!;
    
    [ForeignKey(nameof(Scene))]
    public Guid SceneId { get; set; }
    
    public virtual Scene Scene { get; set; } = null!;
}
