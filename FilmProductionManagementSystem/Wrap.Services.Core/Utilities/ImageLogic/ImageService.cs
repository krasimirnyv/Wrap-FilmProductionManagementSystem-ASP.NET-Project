namespace Wrap.Services.Core.Utilities.ImageLogic;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

using Interfaces;

using static GCommon.DataFormat;
using static GCommon.OutputMessages;
using static GCommon.ApplicationConstants;

public class ImageService(IWebHostEnvironment environment) : IImageService
{
    public async Task<string> SaveImageAsync(IFormFile? photo, IVariantImageStrategy strategy, CancellationToken cancellationToken = default)
    {
        if (photo is null || photo.Length <= 0)
            return strategy.DefaultPath;

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
                Size = new Size(strategy.Width, strategy.Height),
                Mode = strategy.ResizeMode,
                Position = strategy.AnchorPosition
            };

            image.Mutate(operation => operation.Resize(resizeOptions));

            string fileName = $"{Guid.NewGuid():N}.webp";
            string wwwrootPath = environment.WebRootPath;
            string uploadsFolder = Path.Combine(wwwrootPath, ImageFolderName, strategy.FolderName);

            Directory.CreateDirectory(uploadsFolder);

            string physicalPath = Path.Combine(uploadsFolder, fileName);

            WebpEncoder encoder = new WebpEncoder
            {
                Quality = strategy.Quality,
                FileFormat = strategy.FileFormat
            };

            await using FileStream output = new FileStream(
                physicalPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: StreamBufferSize,
                useAsync: true);

            await image.SaveAsync(output, encoder, cancellationToken);

            string webPath = $"/{ImageFolderName}/{strategy.FolderName}/{fileName}";
            
            return webPath;
        }
    }

    public async Task<string> ReplaceAsync(string? currentWebPath, IFormFile? newPhoto, IVariantImageStrategy strategy, CancellationToken cancellationToken = default)
    {
        if (newPhoto is null || newPhoto.Length <= 0)
            return currentWebPath ?? strategy.DefaultPath;
        
        string newWebPath = await SaveImageAsync(newPhoto, strategy, cancellationToken);

        if (!string.IsNullOrWhiteSpace(currentWebPath) &&
            !string.Equals(currentWebPath, newWebPath, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(currentWebPath, strategy.DefaultPath, StringComparison.OrdinalIgnoreCase))
        {
            await DeleteAsync(currentWebPath, strategy, cancellationToken);
        }
        
        return newWebPath;
    }

    public Task DeleteAsync(string? webPath, IVariantImageStrategy strategy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(webPath))
            return Task.CompletedTask;

        if (string.Equals(webPath, strategy.DefaultPath, StringComparison.OrdinalIgnoreCase))
            return Task.CompletedTask;

        string? physicalPath = TryMapWebPathToPhysicalPath(webPath);
        if (physicalPath is null)
            return Task.CompletedTask;

        if (File.Exists(physicalPath))
            File.Delete(physicalPath);
        
        return Task.CompletedTask;
    }
    
    private string? TryMapWebPathToPhysicalPath(string? webPath)
    {
        if (string.IsNullOrWhiteSpace(webPath))
            return null;

        string wwwrootPath = environment.WebRootPath;
        string relativePath = webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        string physicalPath = Path.Combine(wwwrootPath, relativePath);

        return physicalPath;
    }
}