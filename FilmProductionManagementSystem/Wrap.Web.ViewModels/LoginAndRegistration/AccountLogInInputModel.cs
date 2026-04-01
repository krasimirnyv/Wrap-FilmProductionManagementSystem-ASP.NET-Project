namespace Wrap.Web.ViewModels.LoginAndRegistration;

using System.ComponentModel.DataAnnotations;

using static GCommon.DataFormat;

public class AccountLogInInputModel
{
    [Required]
    [Display(Name = DisplayUsername)]
    public string UserName { get; set; } = null!;
    
    [Required]
    [Display(Name = DisplayPassword)]
    public string Password { get; set; } = null!;
    
    [Required]
    public string Role { get; set; } = null!; // "Crew" or "Cast"

    public bool RememberMe { get; set; }
}