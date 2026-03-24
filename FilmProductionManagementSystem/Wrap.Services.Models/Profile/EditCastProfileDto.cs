namespace Wrap.Services.Models.Profile;

using Microsoft.AspNetCore.Http;

using CommonProperties;

public class EditCastProfileDto : CommonUserInfo
{
    public IFormFile? ProfileImage { get; set; }

    public string? Nickname { get; set; }
    
    // Read-only properties for displaying data to UI (cannot be edited)
    public string? Email { get; set; }
    
    public string? CurrentProfileImagePath { get; set; }
    
    public string? Age { get; set; }
    
    public string? Gender { get; set; }
}