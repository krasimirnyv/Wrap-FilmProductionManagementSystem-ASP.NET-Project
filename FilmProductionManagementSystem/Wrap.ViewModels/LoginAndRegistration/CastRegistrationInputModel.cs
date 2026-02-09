namespace Wrap.ViewModels.LoginAndRegistration;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

using GCommon.Enums;

using static GCommon.EntityConstants.Cast;

/// <summary>
/// Step 1: Complete Registration for Cast (Actor)
/// All required fields in one step
/// </summary>
public class CastRegistrationInputModel
{
    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
    
    [Required]
    [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(NicknameMaxLength)]
    public string? Nickname { get; set; }

    [Required]
    public DateTime BirthDate { get; set; }

    [Required]
    public GenderType Gender { get; set; }

    [Required]
    public IFormFile ProfilePicturePath { get; set; } = null!;
    
    [StringLength(BiographyMaxLength)]
    [DataType(DataType.MultilineText)]
    public string? Biography { get; set; }
}
