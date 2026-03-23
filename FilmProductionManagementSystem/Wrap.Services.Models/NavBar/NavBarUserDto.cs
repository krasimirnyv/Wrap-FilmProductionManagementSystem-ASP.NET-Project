namespace Wrap.Services.Models.NavBar;

public class NavBarUserDto
{
    public string UserName { get; set; } = null!;

    public string ProfileImagePath { get; set; } = null!;
    
    public string Role { get; set; } = null!; // "Crew" / "Cast"
}