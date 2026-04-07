namespace Wrap.Web.ViewModels.FindPeople;

public class FindActorsViewModel
{
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; } = 1;
    
    public int ShowingPages { get; set; } = 9;
    
    public int TotalCount { get; set; }

    public string? Search { get; set; }
    
    public byte? Age { get; set; }
    
    public string? Gender { get; set; }

    public string? ProductionId { get; set; }
    
    public bool CanManageProduction { get; set; }
    
    public IReadOnlyCollection<ActorListViewModel> ActorList { get; set; }
        = new List<ActorListViewModel>();
}