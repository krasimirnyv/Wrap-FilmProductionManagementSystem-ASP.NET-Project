namespace Wrap.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using GCommon.Enums;
using MappingEntities;

using static GCommon.EntityConstants.Scene;

/// <summary>
/// Това entity ще се изпозлва за по-нататък, когато се започне изграждането на план-график възможност в апликацията
/// За момента ще бъде игнориран
/// </summary>
public class Scene
{
    [Key]
    public Guid Id { get; set; }
    
    public int SceneNumber { get; set; }

    [Required]
    public SceneType SceneType { get; set; }

    [Required]
    [Unicode]
    [MaxLength(SceneNameMaxLength)]
    public string SceneName { get; set; } = null!;

    [Required]
    [Unicode]
    [MaxLength(LocationMaxLength)]
    public string Location { get; set; } = null!;

    [Unicode]
    [MaxLength(DescriptionMaxLength)]
    public string? Description { get; set; }
    
    [Required]
    [ForeignKey(nameof(Production))]
    public Guid ProductionId { get; set; }

    public virtual Production Production { get; set; } = null!;
    
    public virtual ICollection<ShootingDayScene> ShootingDayScenes { get; set; }
        = new HashSet<ShootingDayScene>();
    
    public virtual ICollection<SceneCrew> SceneCrewMembers { get; set; }
        = new List<SceneCrew>();

    public virtual ICollection<SceneCast> SceneCastMembers { get; set; }
        = new List<SceneCast>();
}
