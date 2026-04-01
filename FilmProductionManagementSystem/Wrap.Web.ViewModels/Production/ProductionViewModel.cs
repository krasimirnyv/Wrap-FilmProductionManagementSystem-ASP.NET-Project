namespace Wrap.Web.ViewModels.Production;

using static GCommon.ApplicationConstants;

public class ProductionViewModel
{
    public string Id { get; set; } = null!;
    
    public string Title { get; set; } = null!;

    public string ThumbnailPath { get; set; } = null!;

    public string StatusType { get; set; } = null!;
    
    public string StatusAbstractClass { get; set; } = DefaultStatus;
}