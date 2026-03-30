namespace Wrap.Services.Core.Utilities.ImageLogic.Interfaces;

using Microsoft.AspNetCore.Http;

public interface IImageService
{
    /// <summary>
    /// Get the image from the FormFile, along with the photo's strategies,
    /// processes the image, validates it, convert it, saves it to the server
    /// and returns the web path to be stored in the database.
    /// </summary>
    /// <param name="photo">IFormFile?</param>
    /// <param name="strategy">IImageVariantStrategy</param>
    /// <param name="cancellationToken">CancellationToken set as default</param>
    /// <returns>webPath of photo -> "/img/profile/{fileName}"</returns>
    /// <exception cref="NotSupportedException"> for unsupported file extensions and very large image size</exception>
    Task<string> SaveImageAsync(IFormFile? photo, IVariantImageStrategy strategy, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Replaces the current image with the new one, if the current image is not the default one.
    /// If the current image is the default one, it simply saves the new image and returns its web path.
    /// </summary>
    /// <param name="currentWebPath">string?</param>
    /// <param name="newPhoto">IFromFile?</param>
    /// <param name="strategy">IVariantImageStrategy</param>
    /// <param name="cancellationToken">CancellationToken set as default</param>
    /// <returns>webPath of new photo -> "/img/profile/{fileName}"</returns>
    /// <exception cref="NotSupportedException"> for unsupported file extensions and very large image size</exception>
    Task<string> ReplaceAsync(string? currentWebPath, IFormFile? newPhoto, IVariantImageStrategy strategy, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="webPath">string?</param>
    /// <param name="strategy">IVariantImageStrategy</param>
    /// <param name="cancellationToken">CancellationToken set as default</param>
    Task DeleteAsync(string? webPath, IVariantImageStrategy strategy, CancellationToken cancellationToken = default);
}