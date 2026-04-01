namespace Wrap.Infrastructure.Tests;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

using Web.Infrastructure.Extensions;
using Data.Repository;

[TestFixture]
public class DataServiceCollectionExtensionsTests
{
    private static Assembly DataAssembly => typeof(LoginRegisterRepository).Assembly;

    private static Type[] GetRepositoryInterfaces()
        => DataAssembly.GetTypes()
            .Where(type => type is { IsInterface: true } &&
                        type.Name.StartsWith("I", StringComparison.Ordinal) &&
                        type.Name.EndsWith("Repository", StringComparison.Ordinal))
            .ToArray();

    public static IEnumerable<TestCaseData> RepositoryInterfacesCases()
        => GetRepositoryInterfaces()
            .Select(repoInterface => new TestCaseData(repoInterface)
                .SetName($"AddDataRepositories registers {repoInterface.Name} as Scoped"));

    [Test]
    public void AddDataRepositories_WhenCalled_RegistersAllRepositoryInterfaces()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act
        services.AddDataRepositories();

        // Assert
        Type[] repoInterfaces = GetRepositoryInterfaces();
        Assert.That(repoInterfaces.Length, Is.GreaterThan(0), "No IRepository interfaces found in data assembly.");

        foreach (Type repoInterface in repoInterfaces)
        {
            ServiceDescriptor[] matches = services.Where(sd => sd.ServiceType == repoInterface).ToArray();

            Assert.That(matches.Length, Is.EqualTo(1), $"Expected exactly 1 registration for {repoInterface.FullName}");
            Assert.That(matches[0].Lifetime, Is.EqualTo(ServiceLifetime.Scoped), $"Expected Scoped for {repoInterface.FullName}");
            Assert.That(matches[0].ImplementationType, Is.Not.Null, $"Expected concrete implementation type for {repoInterface.FullName}");
        }
    }

    [TestCaseSource(nameof(RepositoryInterfacesCases))]
    public void AddDataRepositories_ForEachRepositoryInterface_RegistersExactlyOnceAsScoped(Type repoInterface)
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act
        services.AddDataRepositories();

        // Assert
        ServiceDescriptor[] matches = services.Where(sd => sd.ServiceType == repoInterface).ToArray();

        Assert.That(matches.Length, Is.EqualTo(1), $"Expected exactly 1 registration for {repoInterface.FullName}");
        Assert.That(matches[0].Lifetime, Is.EqualTo(ServiceLifetime.Scoped), $"Expected Scoped for {repoInterface.FullName}");
        Assert.That(matches[0].ImplementationType, Is.Not.Null, $"Expected concrete implementation type for {repoInterface.FullName}");

        // sanity: implementation should be in the same assembly (по конвенция)
        Assert.That(matches[0].ImplementationType!.Assembly, Is.EqualTo(DataAssembly),
            $"Implementation for {repoInterface.FullName} should be in {DataAssembly.GetName().Name}");
    }

    [Test]
    public void AddDataRepositories_WhenCalledTwice_DoesNotThrow()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act + Assert
        Assert.DoesNotThrow(() =>
        {
            services.AddDataRepositories();
            services.AddDataRepositories();
        });
    }
}