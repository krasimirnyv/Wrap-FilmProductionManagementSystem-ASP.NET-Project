namespace Wrap.Infrastructure.Tests;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

using Data.Repository;
using Data.Seeding;
using Web.Infrastructure.Extensions;

[TestFixture]
public class DataServiceCollectionExtensionsTests
{
    private static Assembly DataAssembly => typeof(LoginRegisterRepository).Assembly;
    private static Assembly SeedingAssembly => typeof(ApplicationRoleSeeder).Assembly;

    private static Type[] GetRepositoryInterfaces()
        => DataAssembly.GetTypes()
            .Where(type => type is { IsInterface: true } &&
                           type.Name.StartsWith("I", StringComparison.Ordinal) &&
                           type.Name.EndsWith("Repository", StringComparison.Ordinal))
            .ToArray();

    private static Type[] GetSeederInterfaces()
        => SeedingAssembly.GetTypes()
            .Where(type => type is { IsInterface: true } &&
                           type.Name.StartsWith("I", StringComparison.Ordinal) &&
                           type.Name.EndsWith("Seeder", StringComparison.Ordinal))
            .ToArray();

    public static IEnumerable<TestCaseData> RepositoryInterfacesCases()
        => GetRepositoryInterfaces()
            .Select(repoInterface => new TestCaseData(repoInterface)
                .SetName($"AddDataRepositories registers {repoInterface.Name} as Scoped"));

    public static IEnumerable<TestCaseData> SeederInterfacesCases()
        => GetSeederInterfaces()
            .Select(seederInterface => new TestCaseData(seederInterface)
                .SetName($"AddApplicationRoleSeeding registers {seederInterface.Name} as Transient"));

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

    [Test]
    public void AddApplicationRoleSeeding_WhenCalled_RegistersAllSeederInterfaces()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act
        services.AddApplicationRoleSeeding();

        // Assert
        Type[] seederInterfaces = GetSeederInterfaces();
        Assert.That(seederInterfaces.Length, Is.GreaterThan(0), "No ISeeder interfaces found in seeding assembly.");

        foreach (Type seederInterface in seederInterfaces)
        {
            ServiceDescriptor[] matches = services.Where(sd => sd.ServiceType == seederInterface).ToArray();

            Assert.That(matches.Length, Is.EqualTo(1), $"Expected exactly 1 registration for {seederInterface.FullName}");
            Assert.That(matches[0].Lifetime, Is.EqualTo(ServiceLifetime.Transient), $"Expected Transient for {seederInterface.FullName}");
            Assert.That(matches[0].ImplementationType, Is.Not.Null, $"Expected concrete implementation type for {seederInterface.FullName}");
        }
    }

    [TestCaseSource(nameof(SeederInterfacesCases))]
    public void AddApplicationRoleSeeding_ForEachSeederInterface_RegistersExactlyOnceAsTransient(Type seederInterface)
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act
        services.AddApplicationRoleSeeding();

        // Assert
        ServiceDescriptor[] matches = services.Where(sd => sd.ServiceType == seederInterface).ToArray();

        Assert.That(matches.Length, Is.EqualTo(1), $"Expected exactly 1 registration for {seederInterface.FullName}");
        Assert.That(matches[0].Lifetime, Is.EqualTo(ServiceLifetime.Transient), $"Expected Transient for {seederInterface.FullName}");
        Assert.That(matches[0].ImplementationType, Is.Not.Null, $"Expected concrete implementation type for {seederInterface.FullName}");

        Assert.That(matches[0].ImplementationType!.Assembly, Is.EqualTo(SeedingAssembly),
            $"Implementation for {seederInterface.FullName} should be in {SeedingAssembly.GetName().Name}");
    }

    [Test]
    public void AddApplicationRoleSeeding_WhenCalledTwice_DoesNotThrow()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act + Assert
        Assert.DoesNotThrow(() =>
        {
            services.AddApplicationRoleSeeding();
            services.AddApplicationRoleSeeding();
        });
    }
}