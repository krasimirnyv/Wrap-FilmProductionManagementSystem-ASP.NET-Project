namespace Wrap.Web.Infrastructure.Extensions;

using Microsoft.Extensions.DependencyInjection;

using Utilities;

public static class WebInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddWebInfrastructure(this IServiceCollection services)
    {
        IServiceCollection generators = services.RegisterByConvention(
            typeof(SlugGenerator).Assembly,
            interfaceFilter: type => type is { IsInterface: true } && (type.Name.StartsWith("I") && type.Name.EndsWith("Generator")),
            lifetime: ServiceLifetime.Singleton);
        
        return generators;
    }
}