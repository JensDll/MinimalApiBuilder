using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

public static class DependencyInjection
{
    public static IServiceCollection AddMinimalApiBuilderEndpoints(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetCallingAssembly();
        return services.RegisterEndpoints(assembly);
    }

    private static IServiceCollection RegisterEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpoints =
            from type in assembly.GetTypes()
            where type.BaseType is not null &&
                  type.BaseType.IsGenericType &&
                  type.BaseType.IsAbstract &&
                  type.BaseType.GetGenericTypeDefinition() == typeof(Endpoint<>)
            select type;

        foreach (Type endpoint in endpoints)
        {
            services.AddScoped(endpoint);
        }

        return services;
    }
}
