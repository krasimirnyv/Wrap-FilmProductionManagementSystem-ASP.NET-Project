namespace Wrap.Infrastructure.Tests;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using Web.Infrastructure.Extensions;
using Services.Core;
using Services.Core.Handlers.Interfaces;
using Services.Core.Utilities.ImageLogic.Interfaces;

[TestFixture]
public class CoreServiceCollectionExtensionsTests
{
    [Test]
    public void AddCoreServices_WhenCalled_RegistersAllServicesResolversProvidersHandlersAndStrategies()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act
        services.AddCoreServices();

        // Assert
        Assembly coreAssembly = typeof(LoginRegisterService).Assembly;
        Type[] allTypes = coreAssembly.GetTypes();

        // 1) I*Service => Scoped, exactly one implementation (RegisterByConvention enforces it)
        Type[] serviceInterfaces = allTypes
            .Where(type => type is { IsInterface: true } &&
                        type.Name.StartsWith("I", StringComparison.Ordinal) &&
                        type.Name.EndsWith("Service", StringComparison.Ordinal))
            .ToArray();

        foreach (Type serviceInterface in serviceInterfaces)
        {
            ServiceDescriptor[] matches = services.Where(sd => sd.ServiceType == serviceInterface).ToArray();

            Assert.That(matches.Length, Is.EqualTo(1), $"Expected exactly 1 registration for {serviceInterface.FullName}");
            Assert.That(matches[0].Lifetime, Is.EqualTo(ServiceLifetime.Scoped), $"Expected Scoped for {serviceInterface.FullName}");
            Assert.That(matches[0].ImplementationType, Is.Not.Null, $"Expected concrete implementation type for {serviceInterface.FullName}");
        }

        // 2) I*Resolver => Scoped
        Type[] resolverInterfaces = allTypes
            .Where(type => type is { IsInterface: true } &&
                        type.Name.StartsWith("I", StringComparison.Ordinal) &&
                        type.Name.EndsWith("Resolver", StringComparison.Ordinal))
            .ToArray();

        foreach (Type resolverInterface in resolverInterfaces)
        {
            ServiceDescriptor[] matches = services.Where(sd => sd.ServiceType == resolverInterface).ToArray();

            Assert.That(matches.Length, Is.EqualTo(1), $"Expected exactly 1 registration for {resolverInterface.FullName}");
            Assert.That(matches[0].Lifetime, Is.EqualTo(ServiceLifetime.Scoped), $"Expected Scoped for {resolverInterface.FullName}");
            Assert.That(matches[0].ImplementationType, Is.Not.Null, $"Expected concrete implementation type for {resolverInterface.FullName}");
        }

        // 3) I*Provider => Singleton
        Type[] providerInterfaces = allTypes
            .Where(type => type is { IsInterface: true } &&
                        type.Name.StartsWith("I", StringComparison.Ordinal) &&
                        type.Name.EndsWith("Provider", StringComparison.Ordinal))
            .ToArray();

        foreach (Type providerInterface in providerInterfaces)
        {
            ServiceDescriptor[] matches = services.Where(sd => sd.ServiceType == providerInterface).ToArray();

            Assert.That(matches.Length, Is.EqualTo(1), $"Expected exactly 1 registration for {providerInterface.FullName}");
            Assert.That(matches[0].Lifetime, Is.EqualTo(ServiceLifetime.Singleton), $"Expected Singleton for {providerInterface.FullName}");
            Assert.That(matches[0].ImplementationType, Is.Not.Null, $"Expected concrete implementation type for {providerInterface.FullName}");
        }

        // 4) Registration handlers: register all closed IRegistrationHandler<T> implemented by concrete classes as Scoped
        Type openGenericHandler = typeof(IRegistrationHandler<>);

        Type[] handlerImplementations = allTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } &&
                        type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericHandler))
            .ToArray();

        foreach (Type handlerImplementation in handlerImplementations)
        {
            Type[] closedInterfaces = handlerImplementation.GetInterfaces()
                .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == openGenericHandler)
                .ToArray();

            foreach (Type closedIface in closedInterfaces)
            {
                ServiceDescriptor[] matches = services
                    .Where(sd => sd.ServiceType == closedIface && sd.ImplementationType == handlerImplementation)
                    .ToArray();

                Assert.That(matches.Length, Is.GreaterThanOrEqualTo(1),
                    $"Expected registration for {closedIface.FullName} -> {handlerImplementation.FullName}");

                Assert.That(matches.All(m => m.Lifetime == ServiceLifetime.Scoped), Is.True,
                    $"Expected Scoped lifetime for {closedIface.FullName} -> {handlerImplementation.FullName}");
            }
        }

        // 5) Image strategies: all concrete IVariantImageStrategy implementations registered as Scoped under IVariantImageStrategy
        Type strategyInterface = typeof(IVariantImageStrategy);

        Type[] strategyImplementations = allTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } &&
                        strategyInterface.IsAssignableFrom(type))
            .ToArray();

        foreach (Type strategyImplementation in strategyImplementations)
        {
            ServiceDescriptor[] matches = services
                .Where(sd => sd.ServiceType == strategyInterface && sd.ImplementationType == strategyImplementation)
                .ToArray();

            Assert.That(matches.Length, Is.GreaterThanOrEqualTo(1),
                $"Expected registration for {strategyInterface.FullName} -> {strategyImplementation.FullName}");

            Assert.That(matches.All(m => m.Lifetime == ServiceLifetime.Scoped), Is.True,
                $"Expected Scoped lifetime for {strategyInterface.FullName} -> {strategyImplementation.FullName}");
        }
    }

    [Test]
    public void AddCoreServices_WhenCalled_IsIdempotentInTheSenseThatItDoesNotThrow()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Act + Assert (важното тук е да не гърми при повторно извикване)
        Assert.DoesNotThrow(() =>
        {
            services.AddCoreServices();
            services.AddCoreServices();
        });
    }
}