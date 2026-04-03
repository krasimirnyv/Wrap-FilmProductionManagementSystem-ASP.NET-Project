namespace Wrap.Web.ViewModels.Production;

public class AllProductionsIndexViewModel
{
    public int PageNumber { get; set; } = 1;

    public int TotalPages { get; set; } = 1;

    public int ShowingPages { get; set; } = 3;
    
    public IReadOnlyCollection<ProductionViewModel> Productions { get; set; } 
        = new List<ProductionViewModel>();
    
    public int Count => Productions.Count;
    
    public int TotalCount { get; set; }
    
    public string? SelectedStatus { get; set; }
    
    public bool? IsActive { get; set; }
}