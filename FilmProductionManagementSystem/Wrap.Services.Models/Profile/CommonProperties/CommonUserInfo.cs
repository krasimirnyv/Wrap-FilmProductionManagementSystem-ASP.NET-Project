namespace Wrap.Services.Models.Profile.CommonProperties;

public class CommonUserInfo
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;

    public string? Biography { get; set; }
}