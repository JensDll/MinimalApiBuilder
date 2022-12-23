using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

public static class DependencyInjection
{
    public static IServiceCollection AddMinimalApiBuilderEndpoints(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();
        return services.RegisterEndpoints(assembly).RegisterValidators(assembly);
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

        foreach (var endpoint in endpoints)
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
            if (!validator.TryGetAttribute<RegisterValidatorAsDependencyAttribute>(out var attribute))
            {
                continue;
            }

            var service = typeof(IValidator<>).MakeGenericType(validated);

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (attribute.Lifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient(service, validator);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(service, validator);
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(service, validator);
                    break;
            }
        }

        return services;
    }
}
