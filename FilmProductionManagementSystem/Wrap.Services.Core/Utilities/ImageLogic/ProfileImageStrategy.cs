namespace Wrap.Services.Core.Utilities.ImageLogic;

using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

using Interfaces;

using static GCommon.DataFormat;

public class ProfileImageStrategy : IVariantImageStrategy
{
    public string FolderName => ProfileFolderName;
    
    public string DefaultPath => DefaultProfilePath;
    
    public ushort Width => OutputSizeProfileImage;
    
    public ushort Height => OutputSizeProfileImage;
    
    public ushort Quality => WebpQuality;
    
    public ResizeMode ResizeMode => ResizeMode.Crop;
    
    public AnchorPositionMode AnchorPosition => AnchorPositionMode.Center;
    
    public WebpFileFormatType FileFormat => WebpFileFormatType.Lossy;
}