namespace Wrap.ViewModels.LoginAndRegistration;

using System.ComponentModel.DataAnnotations;

public class AccountLogInInputModel
{
    [Required]
    [Display(Name = "username")]
    public string UserName { get; set; } = null!;
    
    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;
    
    [Required]
    public string Role { get; set; } = null!; // "Crew" or "Cast"

    public bool RememberMe { get; set; } = false;
}