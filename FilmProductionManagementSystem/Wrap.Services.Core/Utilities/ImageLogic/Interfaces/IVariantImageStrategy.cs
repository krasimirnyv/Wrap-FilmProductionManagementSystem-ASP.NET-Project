namespace Wrap.Services.Core.Utilities.ImageLogic.Interfaces;

using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

public interface IVariantImageStrategy
{
    string FolderName { get; }
    
    string DefaultPath { get; }
    
    ushort Width { get; }
    
    ushort Height { get; }
    
    ushort Quality { get; }
    
    ResizeMode ResizeMode { get; }
    
    AnchorPositionMode AnchorPosition { get; }
    
    public WebpFileFormatType FileFormat { get; }
}
