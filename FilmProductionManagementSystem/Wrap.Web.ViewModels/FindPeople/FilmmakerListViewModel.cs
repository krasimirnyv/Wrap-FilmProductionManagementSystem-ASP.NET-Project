namespace Wrap.Web.ViewModels.FindPeople;

public class FilmmakerListViewModel
{
    public string CrewId { get; set; } = null!;
    
    public string ProfileImagePath { get; set; } = null!;
    
    public string FullName { get; set; } = null!;
    
    public string? Nickname { get; set; }
    
    public string? Department { get; set; }
    
    public string? TopRole { get; set; }

    public bool IsAlreadyInProduction { get; set; }
}