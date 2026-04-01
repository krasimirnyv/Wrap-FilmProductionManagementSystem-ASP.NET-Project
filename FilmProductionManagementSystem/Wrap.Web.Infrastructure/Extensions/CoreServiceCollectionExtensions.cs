namespace Wrap.Web.Infrastructure.Extensions;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Services.Core;
using Services.Core.Handlers.Interfaces;
using Services.Core.Utilities.ImageLogic.Interfaces;

public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        Assembly coreAssembly = typeof(LoginRegisterService).Assembly;

        services.RegisterByConvention(
            coreAssembly,
            interfaceFilter: type => type is { IsInterface: true } && (type.Name.StartsWith("I") && type.Name.EndsWith("Service")),
            lifetime: ServiceLifetime.Scoped);

        services.RegisterByConvention(
            coreAssembly,
            interfaceFilter: type => type is { IsInterface: true } && (type.Name.StartsWith("I") && type.Name.EndsWith("Resolver")),
            lifetime: ServiceLifetime.Scoped);

        services.RegisterByConvention(
            coreAssembly,
            interfaceFilter: type => type is { IsInterface: true } && (type.Name.StartsWith("I") && type.Name.EndsWith("Provider")),
            lifetime: ServiceLifetime.Singleton);

        services.RegisterRegistrationHandlers(coreAssembly);
        services.RegisterImageStrategies(coreAssembly);

        return services;
    }

    private static IServiceCollection RegisterRegistrationHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type openGenericHandlerInterface = typeof(IRegistrationHandler<>);

        Type[] implementationTypes = assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } &&
                           type.GetInterfaces()
                               .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericHandlerInterface))
            .ToArray();

        foreach (Type implementationType in implementationTypes)
        {
            Type[] closedInterfaces = implementationType
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericHandlerInterface)
                .ToArray();

            foreach (Type closedInterface in closedInterfaces)
            {
                services.AddScoped(closedInterface, implementationType);
            }
        }

        return services;
    }

    private static IServiceCollection RegisterImageStrategies(this IServiceCollection services, Assembly assembly)
    {
        Type strategyInterfaceType = typeof(IVariantImageStrategy);

        Type[] strategyImplementationTypes = assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && 
                           strategyInterfaceType.IsAssignableFrom(type))
            .ToArray();

        foreach (Type strategyImplementationType in strategyImplementationTypes)
        {
            services.AddScoped(strategyInterfaceType, strategyImplementationType);
        }

        return services;
    }
}