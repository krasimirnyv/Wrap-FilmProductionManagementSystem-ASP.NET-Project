namespace Wrap.Services.Models.FindPeople;

public class ActorListDto
{
    public Guid CastId { get; set; }
    
    public string ProfileImagePath { get; set; } = null!;
    
    public string FullName { get; set; } = null!;
    
    public string? Nickname { get; set; }

    public byte Age { get; set; }

    public string Gender { get; set; } = null!;

    public bool IsAlreadyInProduction { get; set; }
}