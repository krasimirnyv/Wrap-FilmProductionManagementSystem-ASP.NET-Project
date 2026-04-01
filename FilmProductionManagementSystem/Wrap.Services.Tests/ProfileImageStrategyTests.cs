namespace Wrap.Services.Tests;

using NUnit.Framework;

using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

using Core.Utilities.ImageLogic;

using static GCommon.DataFormat;

[TestFixture]
public class ProfileImageStrategyTests
{
    [Test]
    public void Properties_WhenAccessed_ReturnExpectedConfigurationValues()
    {
        // Arrange
        ProfileImageStrategy strategy = new ProfileImageStrategy();

        // Assert
        Assert.That(strategy.FolderName, Is.EqualTo(ProfileFolderName));
        Assert.That(strategy.DefaultPath, Is.EqualTo(DefaultProfilePath));

        Assert.That(strategy.Width, Is.EqualTo(OutputSizeProfileImage));
        Assert.That(strategy.Height, Is.EqualTo(OutputSizeProfileImage));

        Assert.That(strategy.Quality, Is.EqualTo(WebpQuality));

        Assert.That(strategy.ResizeMode, Is.EqualTo(ResizeMode.Crop));
        Assert.That(strategy.AnchorPosition, Is.EqualTo(AnchorPositionMode.Center));
        Assert.That(strategy.FileFormat, Is.EqualTo(WebpFileFormatType.Lossy));
    }
}