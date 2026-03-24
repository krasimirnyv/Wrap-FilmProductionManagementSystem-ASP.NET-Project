namespace Wrap.Services.Models.Profile;

using Microsoft.AspNetCore.Http;

using CommonProperties;

public class EditCrewProfileDto : CommonUserInfo
{
    public IFormFile? ProfileImage { get; set; }
    
    public string? Nickname { get; set; }
    
    // Read-only properties for displaying data to UI (cannot be edited)
    public string? Email { get; set; }
    
    public string? CurrentProfileImagePath { get; set; }
}