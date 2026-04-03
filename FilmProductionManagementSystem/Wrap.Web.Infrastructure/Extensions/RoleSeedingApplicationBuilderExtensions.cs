namespace Wrap.Web.Infrastructure.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Data.Seeding.Interfaces;

public static class RoleSeedingApplicationBuilderExtensions
{
    public static IApplicationBuilder UseRolesSeeder(this IApplicationBuilder applicationBuilder)
    {
        using IServiceScope scope = applicationBuilder
            .ApplicationServices
            .CreateScope();

        IApplicationRoleSeeder roleSeeder = scope
            .ServiceProvider
            .GetRequiredService<IApplicationRoleSeeder>();
        
        roleSeeder
            .SeedRolesAsync()
            .GetAwaiter()
            .GetResult();
        
        return applicationBuilder;
    }
}