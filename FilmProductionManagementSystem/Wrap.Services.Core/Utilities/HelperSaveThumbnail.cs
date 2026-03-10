namespace Wrap.Services.Core.Utilities;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using static GCommon.DataFormat;
using static GCommon.OutputMessages;
using static GCommon.ApplicationConstants;

internal static class HelperSaveThumbnail
{
    /// <summary>
    /// Get the thumbnail image from the form,
    /// validate it, save it to the server
    /// and return the web path to be stored in the database.
    /// </summary>
    /// <param name="environment">IWebHostEnvironment</param>
    /// <param name="photo">IFormFile?</param>
    /// <returns>webPath of photo -> "/img/thumbnail/{fileName}"</returns>
    /// <exception cref="NotSupportedException"> for unsupported file extensions and very large image size</exception>
    internal static async Task<string> SaveThumbnailAsync(IWebHostEnvironment environment, IFormFile? photo)
    {
        if (photo is null || photo.Length <= 0)
            return DefaultThumbnailPath;

        string fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(fileExtension))
            throw new NotSupportedException(string.Format(NotSupportedFileExtension, fileExtension));

        if (photo.Length > MaxFileSize)
            throw new NotSupportedException(string.Format(ExceededFileSizeLimit, MaxFileSize));
        
        string fileName = $"{Guid.NewGuid()}{fileExtension}";
        
        string wwwrootPath = environment.WebRootPath;
        string uploadsFolder = Path.Combine(wwwrootPath, ImageFolderName, ThumbnailFolderName);

        Directory.CreateDirectory(uploadsFolder);
        
        string physicalPath = Path.Combine(uploadsFolder, fileName);
        string webPath = $"/{ImageFolderName}/{ThumbnailFolderName}/{fileName}";
        
        await using (FileStream fileStream = new FileStream(physicalPath, FileMode.Create))
        {
            await photo.CopyToAsync(fileStream);
        }

        return webPath;
    }
}