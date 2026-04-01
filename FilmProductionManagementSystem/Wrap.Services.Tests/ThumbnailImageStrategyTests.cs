namespace Wrap.Services.Tests;

using NUnit.Framework;

using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

using Core.Utilities.ImageLogic;

using static GCommon.DataFormat;

[TestFixture]
public class ThumbnailImageStrategyTests
{
    [Test]
    public void Properties_WhenAccessed_ReturnExpectedConfigurationValues()
    {
        // Arrange
        ThumbnailImageStrategy strategy = new ThumbnailImageStrategy();

        // Assert
        Assert.That(strategy.FolderName, Is.EqualTo(ThumbnailFolderName));
        Assert.That(strategy.DefaultPath, Is.EqualTo(DefaultThumbnailPath));

        Assert.That(strategy.Width, Is.EqualTo(OutputSizeThumbnailWidth));
        Assert.That(strategy.Height, Is.EqualTo(OutputSizeThumbnailHeight));

        Assert.That(strategy.Quality, Is.EqualTo(WebpQuality));

        Assert.That(strategy.ResizeMode, Is.EqualTo(ResizeMode.Crop));
        Assert.That(strategy.AnchorPosition, Is.EqualTo(AnchorPositionMode.Center));
        Assert.That(strategy.FileFormat, Is.EqualTo(WebpFileFormatType.Lossy));
    }
}