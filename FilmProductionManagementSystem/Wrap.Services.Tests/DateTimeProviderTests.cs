namespace Wrap.Services.Tests;

using NUnit.Framework;

using Core.Utilities.Providers;
using Core.Utilities.Providers.Interfaces;

[TestFixture]
public class DateTimeProviderTests
{
    [Test]
    public void Now_WhenCalled_ReturnsTimeBetweenBeforeAndAfter()
    {
        // Arrange
        IDateTimeProvider provider = new DateTimeProvider();

        DateTime before = DateTime.Now;

        // Act
        DateTime now = provider.Now;

        DateTime after = DateTime.Now;

        // Assert
        Assert.That(now, Is.GreaterThanOrEqualTo(before));
        Assert.That(now, Is.LessThanOrEqualTo(after));
    }
}