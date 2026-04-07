namespace Wrap.Web.ViewModels.FindPeople;

using System.ComponentModel.DataAnnotations;

public class AddFilmmakerViewModel
{
    [Required]
    public string ProductionId { get; set; } = null!;

    [Required]
    public string CrewId { get; set; } = null!;

    [Required]
    public string RoleType { get; set; } = null!;
}