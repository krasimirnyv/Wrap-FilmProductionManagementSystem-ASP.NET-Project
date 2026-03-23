namespace Wrap.Infrastructure.Extensions;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

public static class WebApplicationBuilderExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection serviceCollection, Type repositoryType)
    {
        Assembly repositoriesAssembly = repositoryType.Assembly;

        IEnumerable<Type> repositoryInterfaces = repositoriesAssembly
            .GetTypes()
            .Where(t => t.IsInterface && 
                             t.Name.StartsWith("I") && t.Name.EndsWith("Repository"))
            .ToArray();
        
        foreach (Type serviceType in repositoryInterfaces)
        {
            Type implementationType = repositoriesAssembly
                .GetTypes()
                .Single(t => t is { IsClass: true, IsAbstract: false } && 
                                        serviceType.IsAssignableFrom(t));

            serviceCollection.AddScoped(serviceType, implementationType);
        }

        return serviceCollection;
    }

    public static IServiceCollection RegisterUserServices(this IServiceCollection serviceCollection, Type serviceType)
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
}