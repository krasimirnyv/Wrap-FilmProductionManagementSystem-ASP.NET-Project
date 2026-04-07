namespace Wrap.Services.Models.FindPeople;

public class FilmmakerListDto
{
    public Guid CrewId { get; set; }
    
    public string ProfileImagePath { get; set; } = null!;
    
    public string FullName { get; set; } = null!;
    
    public string? Nickname { get; set; }
    
    public string? Role { get; set; }

    public bool IsAlreadyInProduction { get; set; }
}