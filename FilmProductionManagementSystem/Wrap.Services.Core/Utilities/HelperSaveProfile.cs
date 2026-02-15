namespace Wrap.Services.Core.Utilities;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using static GCommon.ApplicationConstants;

internal static class  HelperSaveProfile
{
    /// <summary>
    /// Get the profile image from the form,
    /// validate it, save it to the server
    /// and return the web path to be stored in the database.
    /// </summary>
    /// <param name="environment">IWebHostEnvironment</param>
    /// <param name="photo">IFormFile?</param>
    /// <returns>webPath of photo -> "/img/profile/{fileName}"</returns>
    /// <exception cref="NotSupportedException"> for unsupported file extensions and very large image size</exception>
    internal static async Task<string> SaveProfileImageAsync(IWebHostEnvironment environment, IFormFile? photo)
    {
        if (photo is null || photo.Length <= 0)
            return "/img/profile/default-profile.png";

        string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif", ".heif", ".heic", ".hif"];
        string fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
            throw new NotSupportedException($"The file extension {fileExtension} is not supported.");

        if (photo.Length > MaxFileSize)
            throw new NotSupportedException($"The file size limit {MaxFileSize} exceeded.");
        
        string fileName = $"{Guid.NewGuid()}{fileExtension}";
        
        string wwwrootPath = environment.WebRootPath;
        string uploadsFolder = Path.Combine(wwwrootPath, "img", "profile");

        Directory.CreateDirectory(uploadsFolder);
        
        string physicalPath = Path.Combine(uploadsFolder, fileName);
        string webPath = $"/img/profile/{fileName}";
        
        await using (FileStream fileStream = new FileStream(physicalPath, FileMode.Create))
        {
            await photo.CopyToAsync(fileStream);
        }

        return webPath;
    }
}