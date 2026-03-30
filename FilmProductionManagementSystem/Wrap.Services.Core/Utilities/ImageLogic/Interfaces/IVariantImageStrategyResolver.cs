namespace Wrap.Services.Core.Utilities.ImageLogic.Interfaces;

public interface IVariantImageStrategyResolver
{
    /// <summary>
    /// Resolves current image strategy that is used for handling
    /// the image upload and processing logic for a specific folder.
    /// </summary>
    /// <param name="folderName">string</param>
    /// <returns>specified strategy wrapped in IVariantImageStrategy</returns>
    IVariantImageStrategy Resolve(string folderName);
}