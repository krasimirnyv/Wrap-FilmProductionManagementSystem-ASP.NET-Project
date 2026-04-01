namespace Wrap.Web.ViewModels.Profile;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

using static GCommon.EntityConstants.Cast;

public class EditCastProfileInputModel
{
    [Required]
    [StringLength(FirstNameMaxLength, MinimumLength = LastNameMinLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength)]
    public string LastName { get; set; } = null!;

    [StringLength(NicknameMaxLength)]
    public string? Nickname { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(BiographyMaxLength)]
    public string? Biography { get; set; }

    public IFormFile? ProfileImage { get; set; }

    // Read-only properties for displaying data to UI (cannot be edited)
    public string? Email { get; set; }
    
    public string? CurrentProfileImagePath { get; set; }
    
    public string? Age { get; set; }
    
    public string? Gender { get; set; }
}
