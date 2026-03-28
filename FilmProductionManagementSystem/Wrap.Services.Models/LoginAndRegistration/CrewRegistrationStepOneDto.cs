namespace Wrap.Services.Models.LoginAndRegistration;

using Microsoft.AspNetCore.Http;

public class CrewRegistrationStepOneDto
{
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Nickname { get; set; }

    public IFormFile? ProfilePicture { get; set; }
    
    public string? Biography { get; set; }
}