namespace Wrap.Infrastructure.Tests;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Web.Infrastructure.Extensions;
using Web.Infrastructure.Utilities;

[TestFixture]
public class WebInfrastructureServiceCollectionExtensionsTests
{
    private static Assembly WebInfraAssembly => typeof(SlugGenerator).Assembly;

    private static Type[] GetGeneratorInterfaces()
        => WebInfraAssembly.GetTypes()
            .Where(type => type is { IsInterface: true } &&
                        type.Name.StartsWith("I", StringComparison.Ordinal) &&
                        type.Name.EndsWith("Generator", StringComparison.Ordinal))
            .ToArray();

    public static IEnumerable<TestCaseData> GeneratorInterfacesCases()
        => GetGeneratorInterfaces()
            .Select(generatorInterface => new TestCaseData(generatorInterface)
                .SetName($"AddWebInfrastructure registers {generatorInterface.Name} as Singleton"));

    [Test]
    public void AddWebInfrastructure_WhenCalled_RegistersAllGeneratorInterfaces()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act
        services.AddWebInfrastructure();

        // Assert
        Type[] generatorInterfaces = GetGeneratorInterfaces();
        Assert.That(generatorInterfaces.Length, Is.GreaterThan(0), "No I*Generator interfaces found in web infrastructure assembly.");

        foreach (Type generatorInterface in generatorInterfaces)
        {
            ServiceDescriptor[] matches = services.Where(sd => sd.ServiceType == generatorInterface).ToArray();

            Assert.That(matches.Length, Is.EqualTo(1), $"Expected exactly 1 registration for {generatorInterface.FullName}");
            Assert.That(matches[0].Lifetime, Is.EqualTo(ServiceLifetime.Singleton), $"Expected Singleton for {generatorInterface.FullName}");
            Assert.That(matches[0].ImplementationType, Is.Not.Null, $"Expected concrete implementation type for {generatorInterface.FullName}");
        }
    }

    [TestCaseSource(nameof(GeneratorInterfacesCases))]
    public void AddWebInfrastructure_ForEachGeneratorInterface_RegistersExactlyOnceAsSingleton(Type generatorInterface)
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act
        services.AddWebInfrastructure();

        // Assert
        ServiceDescriptor[] matches = services.Where(sd => sd.ServiceType == generatorInterface).ToArray();

        Assert.That(matches.Length, Is.EqualTo(1), $"Expected exactly 1 registration for {generatorInterface.FullName}");
        Assert.That(matches[0].Lifetime, Is.EqualTo(ServiceLifetime.Singleton), $"Expected Singleton for {generatorInterface.FullName}");
        Assert.That(matches[0].ImplementationType, Is.Not.Null, $"Expected concrete implementation type for {generatorInterface.FullName}");

        // sanity: implementation should be in the same assembly
        Assert.That(matches[0].ImplementationType!.Assembly, Is.EqualTo(WebInfraAssembly),
            $"Implementation for {generatorInterface.FullName} should be in {WebInfraAssembly.GetName().Name}");
    }

    [Test]
    public void AddWebInfrastructure_WhenCalledTwice_DoesNotThrow()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act + Assert
        Assert.DoesNotThrow(() =>
        {
            services.AddWebInfrastructure();
            services.AddWebInfrastructure();
        });
    }
}