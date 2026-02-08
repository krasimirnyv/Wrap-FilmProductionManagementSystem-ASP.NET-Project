namespace Wrap.ViewModels.LoginAndRegistration;

using static Wrap.GCommon.EntityConstants.Crew;

public class CrewRegistrationStepOneDraft
{
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Nickname { get; set; }

    public string? ProfilePicturePath { get; set; } = DefaultProfileImagePath;
    
    public string? Biography { get; set; }
}