namespace Wrap.Infrastructure.Tests;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Web.Infrastructure.Extensions;

[TestFixture]
public class ConventionRegistrationExtensionsTests
{
    private interface INoImpl { }

    private interface ISingleImpl { }
    
    private class SingleImpl : ISingleImpl { }

    private interface IMultiImpl { }
    
    private class MultiImplA : IMultiImpl { }
    
    private class MultiImplB : IMultiImpl { }

    private interface IAbstractOnly { }
    
    public abstract class AbstractOnlyImpl : IAbstractOnly { }
    
    private static readonly TestCaseData[] SkippedInterfaces =
    [
        new(typeof(INoImpl)),
        new(typeof(IAbstractOnly)),
    ];

    private static readonly TestCaseData[] RegisteredInterfaces =
    [
        new(typeof(ISingleImpl), typeof(SingleImpl)),
    ];
    
    [TestCaseSource(nameof(SkippedInterfaces))]
    public void RegisterByConvention_WhenNoConcreteImplementation_DoesNotRegister(Type interfaceType)
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Act
        services.RegisterByConvention
        (
            assembly: assembly,
            interfaceFilter: type => type == interfaceType,
            lifetime: ServiceLifetime.Scoped
        );

        // Assert
        ServiceDescriptor? descriptor = services.SingleOrDefault(sd => sd.ServiceType == interfaceType);
        Assert.That(descriptor, Is.Null);
    }

    [TestCaseSource(nameof(RegisteredInterfaces))]
    public void RegisterByConvention_WhenExactlyOneImplementation_RegistersDescriptor(Type interfaceType, Type implementationType)
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        Assembly assembly = Assembly.GetExecutingAssembly();
        ServiceLifetime lifetime = ServiceLifetime.Singleton;

        // Act
        services.RegisterByConvention
        (
            assembly: assembly,
            interfaceFilter: type => type == interfaceType,
            lifetime: lifetime
        );

        // Assert
        ServiceDescriptor? descriptor = services.SingleOrDefault(sd => sd.ServiceType == interfaceType);

        Assert.That(descriptor, Is.Not.Null);
        Assert.That(descriptor.ImplementationType, Is.EqualTo(implementationType));
        Assert.That(descriptor.Lifetime, Is.EqualTo(lifetime));
    }

    [Test]
    public void RegisterByConvention_WhenMultipleImplementations_ThrowsInvalidOperationException()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
            services.RegisterByConvention
            (
                assembly: assembly,
                interfaceFilter: type => type == typeof(IMultiImpl),
                lifetime: ServiceLifetime.Transient
            ));
    }

    [Test]
    public void RegisterByConvention_WhenFilterMatchesNothing_DoesNotThrow_AndReturnsSameServices()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Act
        IServiceCollection returned = services.RegisterByConvention(
            assembly: assembly,
            interfaceFilter: _ => false,
            lifetime: ServiceLifetime.Scoped);

        // Assert
        Assert.That(returned, Is.SameAs(services));
        Assert.That(services, Is.Empty);
    }
}