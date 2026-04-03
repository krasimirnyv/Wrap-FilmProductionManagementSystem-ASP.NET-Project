using Wrap.Data.Seeding;

namespace Wrap.Web.Infrastructure.Extensions;

using Microsoft.Extensions.DependencyInjection;

using Data.Repository;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        IServiceCollection repositories = services
            .RegisterByConvention(
                typeof(LoginRegisterRepository).Assembly, 
                interfaceFilter: type => type is { IsInterface: true } && (type.Name.StartsWith("I") && type.Name.EndsWith("Repository")), 
                lifetime: ServiceLifetime.Scoped);

        return repositories;
    }

    public static IServiceCollection AddApplicationRoleSeeding(this IServiceCollection services)
    {
        IServiceCollection identitySeeder = services
            .RegisterByConvention(
                typeof(ApplicationRoleSeeder).Assembly,
                interfaceFilter: type =>
                    type is { IsInterface: true } && (type.Name.StartsWith("I") && type.Name.EndsWith("Seeder")),
                lifetime: ServiceLifetime.Transient);
        
        return identitySeeder;
    }
}