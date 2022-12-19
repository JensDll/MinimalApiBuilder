using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

public static class DependencyInjection
{
    public static void AddEndpoints(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetCallingAssembly();
        services.RegisterEndpoints(assembly).RegisterValidators(assembly);
    }

    private static IServiceCollection RegisterEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpoints =
            from type in assembly.GetTypes()
            where type.IsSubclassOf(typeof(EndpointBase))
            select type;

        foreach (Type endpoint in endpoints)
        {
            services.AddScoped(endpoint);
        }

        return services;
    }

    private static IServiceCollection RegisterValidators(this IServiceCollection services, Assembly assembly)
    {
        var validators =
            from type in assembly.GetTypes()
            where type.BaseType is not null &&
                  type.BaseType.IsGenericType &&
                  type.BaseType.IsAbstract &&
                  type.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>)
            select (validator: type, validated: type.BaseType!.GetGenericArguments()[0]);

        foreach (var (validator, validated) in validators)
        {
            services.AddSingleton(typeof(IValidator<>).MakeGenericType(validated), validator);
        }

        return services;
    }
}
