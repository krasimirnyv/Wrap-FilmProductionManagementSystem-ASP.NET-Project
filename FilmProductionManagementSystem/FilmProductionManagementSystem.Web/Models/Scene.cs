namespace FilmProductionManagementSystem.Web.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Enums;

using static Common.EntityConstants.Scene;

public class Scene
{
    [Key]
    public Guid Id { get; set; }

    public int SceneNumber { get; set; }

    [Required]
    public SceneType SceneType { get; set; }

    [Required]
    [MaxLength(SceneNameMaxLength)]
    public string SceneName { get; set; } = null!;

    [Required]
    [MaxLength(LocationMaxLength)]
    public string Location { get; set; } = null!;

    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }
    
    public virtual ShootingDayScene? ShootingDayScene { get; set; }
    
    [Required]
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }

    public virtual Production Production { get; set; } = null!;
    
    public virtual ICollection<Crew> CrewMembers { get; set; }
        = new List<Crew>();

    public virtual ICollection<Cast> CastMembers { get; set; }
        = new List<Cast>();
}
