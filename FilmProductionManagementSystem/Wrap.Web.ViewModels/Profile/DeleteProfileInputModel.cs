namespace Wrap.Web.ViewModels.Profile;

using System.ComponentModel.DataAnnotations;

public class DeleteProfileInputModel
{
    [Required]
    public string Password { get; set; } = null!;
    
    [Required]
    public bool IsConfirmed { get; set; }
    
    // Read-only properties
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public string ProfileImagePath { get; set; } = null!;
    
    public string UserName { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;

    public int ProductionsCount { get; set; }
    
    public int ScenesCount { get; set; }
    
    public int? SkillsCount { get; set; }
}