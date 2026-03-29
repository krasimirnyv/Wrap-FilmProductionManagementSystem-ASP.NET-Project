namespace Wrap.Infrastructure.Extensions;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Data.Repository;
using Services.Core;
using Services.Core.Handlers;
using Services.Core.Handlers.Interfaces;
using Utilities;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.RegisterRepositories(typeof(LoginRegisterRepository));
        serviceCollection.RegisterUserServices(typeof(LoginRegisterService));
        
        serviceCollection.RegisterRegistrationHandlers(typeof(CrewRegistrationHandler));
        serviceCollection.RegisterResolvers(typeof(RegistrationHandlerResolver));
        
        serviceCollection.RegisterGenerators(typeof(SlugGenerator));

        return serviceCollection;
    }

    private static IServiceCollection RegisterRepositories(this IServiceCollection serviceCollection, Type repositoryType)
    {
        Assembly repositoriesAssembly = repositoryType.Assembly;

        IEnumerable<Type> repositoryInterfaces = repositoriesAssembly
            .GetTypes()
            .Where(t => t.IsInterface &&
                        t.Name.StartsWith("I") && t.Name.EndsWith("Repository"))
            .ToArray();

        foreach (Type currentRepositoryType in repositoryInterfaces)
        {
            Type implementationType = repositoriesAssembly
                .GetTypes()
                .Single(t => t is { IsClass: true, IsAbstract: false } &&
                             currentRepositoryType.IsAssignableFrom(t));

            serviceCollection.AddScoped(currentRepositoryType, implementationType);
        }

        return serviceCollection;
    }

    private static IServiceCollection RegisterUserServices(this IServiceCollection serviceCollection, Type serviceType)
    {
        Assembly servicesAssembly = serviceType.Assembly;

        IEnumerable<Type> serviceInterfaces = servicesAssembly
            .GetTypes()
            .Where(t => t.IsInterface &&
                        t.Name.StartsWith("I") && t.Name.EndsWith("Service"))
            .ToArray();

        foreach (Type currentServiceType in serviceInterfaces)
        {
            Type implementationType = servicesAssembly
                .GetTypes()
                .Single(t => t is { IsClass: true, IsAbstract: false } &&
                             currentServiceType.IsAssignableFrom(t));

            serviceCollection.AddScoped(currentServiceType, implementationType);
        }

        return serviceCollection;
    }

    private static IServiceCollection RegisterRegistrationHandlers(this IServiceCollection serviceCollection, Type handlerType)
    {
        Assembly handlersAssembly = handlerType.Assembly;

        Type openGenericHandlerInterfaceType = typeof(IRegistrationHandler<>);

        IEnumerable<Type> handlerImplementationTypes = handlersAssembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        t.GetInterfaces()
                            .Any(i => i.IsGenericType &&
                                      i.GetGenericTypeDefinition() == openGenericHandlerInterfaceType))
            .ToArray();

        foreach (Type implementationType in handlerImplementationTypes)
        {
            IEnumerable<Type> closedHandlerInterfaces = implementationType
                .GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == openGenericHandlerInterfaceType)
                .ToArray();

            foreach (Type closedInterface in closedHandlerInterfaces)
            {
                serviceCollection.AddScoped(closedInterface, implementationType);
            }
        }

        return serviceCollection;
    }

    private static IServiceCollection RegisterResolvers(this IServiceCollection serviceCollection, Type resolverType)
    {
        Assembly resolversAssembly = resolverType.Assembly;

        IEnumerable<Type> resolverInterfaces = resolversAssembly
            .GetTypes()
            .Where(t => t.IsInterface &&
                        t.Name.StartsWith("I") && t.Name.EndsWith("Resolver"))
            .ToArray();

        foreach (Type currentResolverType in resolverInterfaces)
        {
            Type implementationType = resolversAssembly
                .GetTypes()
                .Single(t => t is { IsClass: true, IsAbstract: false } &&
                             currentResolverType.IsAssignableFrom(t));

            serviceCollection.AddScoped(currentResolverType, implementationType);
        }

        return serviceCollection;
    }
    
    private static IServiceCollection RegisterGenerators(this IServiceCollection serviceCollection, Type generatorType)
    {
        Assembly generatorAssembly = generatorType.Assembly;

        IEnumerable<Type> generatorInterfaces = generatorAssembly
            .GetTypes()
            .Where(t => t.IsInterface &&
                        t.Name.StartsWith("I") && t.Name.EndsWith("Generator"))
            .ToArray();

        foreach (Type currentGeneratorType in generatorInterfaces)
        {
            Type implementationType = generatorAssembly
                .GetTypes()
                .Single(t => t is { IsClass: true, IsAbstract: false } &&
                             currentGeneratorType.IsAssignableFrom(t));

            serviceCollection.AddSingleton(currentGeneratorType, implementationType);
        }

        return serviceCollection;
    }
}