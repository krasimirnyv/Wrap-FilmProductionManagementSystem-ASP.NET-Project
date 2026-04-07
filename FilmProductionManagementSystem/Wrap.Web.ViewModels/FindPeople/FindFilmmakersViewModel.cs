namespace Wrap.Web.ViewModels.FindPeople;

public class FindFilmmakersViewModel
{
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; } = 1;
    
    public int ShowingPages { get; set; } = 9;
    
    public int TotalCount { get; set; }

    public string? Search { get; set; }
    
    public int? RoleType { get; set; }

    public string? ProductionId { get; set; }
    
    public bool CanManageProduction { get; set; }

    public IReadOnlyCollection<FilmmakerListViewModel> FilmmakerList { get; set; }
        = new List<FilmmakerListViewModel>();
}