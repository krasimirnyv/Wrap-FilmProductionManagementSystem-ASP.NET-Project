namespace Wrap.Services.Tests;

using Moq;
using NUnit.Framework;

using Core.Utilities.ImageLogic;
using Core.Utilities.ImageLogic.Interfaces;

using static GCommon.OutputMessages;

[TestFixture]
public class VariantImageStrategyResolverTests
{
    [Test]
    public void Resolve_WhenKeyExists_ReturnsMatchingStrategy()
    {
        // Arrange
        IVariantImageStrategy profileStrategy = CreateStrategy(folderName: "profile");
        IVariantImageStrategy thumbnailStrategy = CreateStrategy(folderName: "thumbnail");

        VariantImageStrategyResolver resolver = new VariantImageStrategyResolver(strategies: [profileStrategy, thumbnailStrategy]);

        // Act
        IVariantImageStrategy result = resolver.Resolve("profile");

        // Assert
        Assert.That(result, Is.SameAs(profileStrategy));
    }

    [Test]
    public void Resolve_WhenKeyDifferentCasing_ReturnsMatchingStrategy_CaseInsensitive()
    {
        // Arrange
        IVariantImageStrategy profileStrategy = CreateStrategy(folderName: "profile");

        VariantImageStrategyResolver resolver = new VariantImageStrategyResolver(strategies: [profileStrategy]);

        // Act
        IVariantImageStrategy result = resolver.Resolve("PrOfIlE");

        // Assert
        Assert.That(result, Is.SameAs(profileStrategy));
    }

    [Test]
    public void Resolve_WhenKeyDoesNotExist_ThrowsNotSupportedExceptionWithMessage()
    {
        // Arrange
        IVariantImageStrategy profileStrategy = CreateStrategy(folderName: "profile");

        VariantImageStrategyResolver resolver = new VariantImageStrategyResolver(strategies: [profileStrategy]);

        const string missingKey = "missing";

        // Act
        NotSupportedException ex = Assert.Throws<NotSupportedException>(
            () => resolver.Resolve(missingKey));

        // Assert
        Assert.That(ex.Message, Does.Contain(string.Format(NoImageStrategyFound, missingKey)));
    }

    [Test]
    public void Constructor_WhenDuplicateKeysDifferentCasing_ThrowsArgumentException()
    {
        // Arrange
        IVariantImageStrategy strategy1 = CreateStrategy(folderName: "profile");
        IVariantImageStrategy strategy2 = CreateStrategy(folderName: "PrOfIlE"); // duplicate under OrdinalIgnoreCase

        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
        {
            VariantImageStrategyResolver variantImageStrategyResolver = new VariantImageStrategyResolver(strategies: [strategy1, strategy2]);
        });
    }

    [Test]
    public void Constructor_WhenStrategiesContainsNull_ThrowsNullReferenceException()
    {
        // Arrange
        IEnumerable<IVariantImageStrategy?> strategies =
        [
            null
        ];

        // Act + Assert
        Assert.Throws<NullReferenceException>(() =>
        {
            VariantImageStrategyResolver variantImageStrategyResolver = new VariantImageStrategyResolver(strategies: strategies.Cast<IVariantImageStrategy>());
        });
    }

    private static IVariantImageStrategy CreateStrategy(string folderName)
    {
        Mock<IVariantImageStrategy> strategyMock = new Mock<IVariantImageStrategy>(MockBehavior.Strict);
        strategyMock.SetupGet(s => s.FolderName).Returns(folderName);

        // останалите пропъртита не са нужни за resolver-а
        return strategyMock.Object;
    }
}