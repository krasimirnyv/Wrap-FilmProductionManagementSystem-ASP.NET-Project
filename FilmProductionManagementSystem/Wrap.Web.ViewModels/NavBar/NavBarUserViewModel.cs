namespace Wrap.ViewModels.NavBar;

public class NavBarUserViewModel
{
    public string UserName { get; set; } = null!;

    public string ProfileImagePath { get; set; } = null!;
    
    public string Role { get; set; } = null!; // "Crew" / "Cast"
}