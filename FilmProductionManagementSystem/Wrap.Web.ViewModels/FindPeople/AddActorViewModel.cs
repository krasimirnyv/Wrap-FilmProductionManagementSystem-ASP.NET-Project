namespace Wrap.Web.ViewModels.FindPeople;

using System.ComponentModel.DataAnnotations;

using static GCommon.EntityConstants.Cast;

public class AddActorViewModel
{
    [Required]
    public string ProductionId { get; set; } = null!;

    [Required]
    public string CastId { get; set; } = null!;

    [Required]
    [StringLength(RoleMaxLength, MinimumLength = RoleMinLength)]
    public string RoleName { get; set; } = null!;
}