namespace Wrap.ViewModels.LoginAndRegistration;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

using static GCommon.EntityConstants.Crew;
/// <summary>
/// Step 1: Basic Information for Crew Registration
/// </summary>
public class CrewRegistrationStepOneInputModel
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
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength)]
    public string LastName { get; set; } = null!;

    [StringLength(NicknameMaxLength)]
    public string? Nickname { get; set; }

    public IFormFile? ProfilePicturePath { get; set; }
    
    [StringLength(BiographyMaxLength)]
    [DataType(DataType.MultilineText)]
    public string? Biography { get; set; }
}
