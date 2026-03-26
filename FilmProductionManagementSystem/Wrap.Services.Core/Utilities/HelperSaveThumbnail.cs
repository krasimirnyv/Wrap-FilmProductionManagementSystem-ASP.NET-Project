namespace Wrap.Services.Core.Utilities;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

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
    /// <param name="cancellationToken">CancellationToken set as default</param>
    /// <returns>webPath of photo -> "/img/thumbnail/{fileName}"</returns>
    /// <exception cref="NotSupportedException"> for unsupported file extensions and very large image size</exception>
    internal static async Task<string> SaveThumbnailAsync(IWebHostEnvironment environment, IFormFile? photo, CancellationToken cancellationToken = default)
    {
        if (photo is null || photo.Length <= 0)
            return DefaultThumbnailPath;

        if (photo.Length > MaxFileSize)
            throw new NotSupportedException(string.Format(ExceededFileSizeLimit, MaxFileSize));
        
        string fileExtension = Path.GetExtension(photo.FileName);

        if (string.IsNullOrWhiteSpace(fileExtension) || !AllowedExtensions.Contains(fileExtension))
            throw new NotSupportedException(string.Format(NotSupportedFileExtension, fileExtension));

        Image image;
        try
        {
            await using Stream input = photo.OpenReadStream();
            image = await Image.LoadAsync(input, cancellationToken);
        }
        catch (UnknownImageFormatException uife)
        {
            throw new NotSupportedException(string.Format(UnsupportedOrCorruptedFormat, uife.Message));
        }
        catch (Exception e)
        {
            throw new NotSupportedException(string.Format(FailedToReadImage, e.Message));
        }

        using (image)
        {
            if (image.Height <= 0 || image.Width <= 0)
                throw new NotSupportedException(string.Format(InvalidImageDimensions, image.Height, image.Width));

            if (image.Height > MaxPixelDimension || image.Width > MaxPixelDimension)
                throw new NotSupportedException(string.Format(ImageTooLarge, MaxPixelDimension, image.Height, image.Width));

            image.Metadata.ExifProfile = null;
            image.Metadata.IccProfile = null;
            image.Metadata.XmpProfile = null;

            ResizeOptions resizeOptions = new ResizeOptions
            {
                Size = new Size(OutputSizeThumbnailWidth, OutputSizeThumbnailHeight),
                Mode = ResizeMode.Crop,
                Position = AnchorPositionMode.Center,
            };

            image.Mutate(operation => operation.Resize(resizeOptions));

            string fileName = $"{Guid.NewGuid():N}.webp";
            string wwwrootPath = environment.WebRootPath;
            string uploadsFolder = Path.Combine(wwwrootPath, ImageFolderName, ThumbnailFolderName);

            Directory.CreateDirectory(uploadsFolder);
            
            string physicalPath = Path.Combine(uploadsFolder, fileName);

            WebpEncoder encoder = new WebpEncoder
            {
                Quality = WebpQuality,
                FileFormat = WebpFileFormatType.Lossy
            };
            
            await using FileStream output = new FileStream(
                physicalPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: StreamBufferSize,
                useAsync: true);
            
            await image.SaveAsync(output, encoder, cancellationToken);
            
            string webPath = $"/{ImageFolderName}/{ThumbnailFolderName}/{fileName}";

            return webPath;
        }
    }
}