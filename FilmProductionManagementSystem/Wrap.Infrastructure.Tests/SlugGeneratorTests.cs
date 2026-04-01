namespace Wrap.Infrastructure.Tests;

using NUnit.Framework;

using Wrap.Web.Infrastructure.Utilities;

[TestFixture]
public class SlugGeneratorTests
{
    private SlugGenerator slugGenerator = null!;

    [SetUp]
    public void SetUp()
    {
        slugGenerator = new SlugGenerator();
    }

    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("   ", "")]
    [TestCase("   Hello World   ", "hello-world")]
    [TestCase("Hello    World\t\tAgain\nHere", "hello-world-again-here")]
    [TestCase("Hello, World! @2026 #Wrap", "hello-world-2026-wrap")]
    [TestCase("hello---world----again", "hello-world-again")]
    [TestCase("   --- Hello World ---   ", "hello-world")]
    [TestCase("!!!@@@###$$$", "")]
    [TestCase("My-Slug_Already-OK", "my-slugalready-ok")]
    public void GenerateSlug_WhenCalled_ReturnsExpected(string? input, string expected)
    {
        // Act
        string result = slugGenerator.GenerateSlug(input!);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}