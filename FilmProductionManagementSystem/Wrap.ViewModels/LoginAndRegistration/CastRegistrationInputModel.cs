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
    [Display(Name = "username")]
    public string UserName { get; set; } = null!;

    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;
    
    [Required]
    [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(NicknameMaxLength)]
    [Display(Name = "Nickname (optional)")]
    public string? Nickname { get; set; }

    [Required]
    [Display(Name = "Date of Birth")]
    public DateTime BirthDate { get; set; }

    [Required]
    [Display(Name = "Gender")]
    public GenderType Gender { get; set; }

    [Required]
    [Display(Name = "Profile Image")]
    public string ProfilePicturePath { get; set; } = null!;
    
    [StringLength(BiographyMaxLength)]
    [Display(Name = "Tell us about yourself")]
    [DataType(DataType.MultilineText)]
    public string? Biography { get; set; }
}
