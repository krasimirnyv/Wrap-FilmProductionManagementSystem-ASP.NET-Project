namespace Wrap.Web.ViewModels.FindPeople;

public class ActorListViewModel
{
    public string CastId { get; set; } = null!;
    
    public string ProfileImagePath { get; set; } = null!;
    
    public string FullName { get; set; } = null!;
    
    public string? Nickname { get; set; }

    public string Age { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public bool IsAlreadyInProduction { get; set; }
}