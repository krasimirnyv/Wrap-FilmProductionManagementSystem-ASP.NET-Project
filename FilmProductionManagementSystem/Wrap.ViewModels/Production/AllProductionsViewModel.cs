namespace Wrap.ViewModels.Production;

using static GCommon.ApplicationConstants;

public class AllProductionsViewModel
{
    public string Id { get; set; } = null!;
    
    public string Title { get; set; } = null!;

    public string? Thumbnail { get; set; }

    public string StatusType { get; set; } = null!;
    
    public string StatusAbstractClass { get; set; } = DefaultStatus;
}