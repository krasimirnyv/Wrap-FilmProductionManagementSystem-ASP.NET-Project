namespace Wrap.ViewModels.LoginAndRegistration;

public class CrewRegistrationStepOneDraft
{
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Nickname { get; set; }

    public string? ProfilePicturePath { get; set; }
    
    public string? Biography { get; set; }
}