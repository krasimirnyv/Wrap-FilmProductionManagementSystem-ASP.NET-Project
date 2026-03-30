namespace Wrap.Services.Core.Utilities.ImageLogic;

using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

using Interfaces;

using static GCommon.DataFormat;

public class ThumbnailImageStrategy : IVariantImageStrategy
{
    public string FolderName => ThumbnailFolderName;
    
    public string DefaultPath => DefaultThumbnailPath;
    
    public ushort Width => OutputSizeThumbnailWidth;
    
    public ushort Height => OutputSizeThumbnailHeight;
    
    public ushort Quality => WebpQuality;

    public ResizeMode ResizeMode => ResizeMode.Crop;
    
    public AnchorPositionMode AnchorPosition => AnchorPositionMode.Center;
    
    public WebpFileFormatType FileFormat => WebpFileFormatType.Lossy;
}