namespace Wrap.Web.Infrastructure.Extensions;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using static GCommon.OutputMessages;

public static class ConventionRegistrationExtensions
{
    public static IServiceCollection RegisterByConvention(this IServiceCollection services, Assembly assembly, Func<Type, bool> interfaceFilter, ServiceLifetime lifetime)
    {
        Type[] allTypes = assembly.GetTypes();

        Type[] interfaceTypes = allTypes
            .Where(interfaceFilter)
            .ToArray();

        foreach (Type interfaceType in interfaceTypes)
        {
            Type[] implementationTypes = allTypes
                .Where(type => type is { IsClass: true, IsAbstract: false } &&
                               interfaceType.IsAssignableFrom(type))
                .ToArray();

            if (implementationTypes.Length == 0)
                continue;

            if (implementationTypes.Length > 1)
                throw new InvalidOperationException(string.Format(MultiImplementationException, interfaceType.FullName, string.Join(", ", implementationTypes.Select(t => t.FullName))));

            services.Add(new ServiceDescriptor(interfaceType, implementationTypes[0], lifetime));
        }

        return services;
    }
}