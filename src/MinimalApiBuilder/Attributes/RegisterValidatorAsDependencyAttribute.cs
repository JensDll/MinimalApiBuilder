using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterValidatorAsDependencyAttribute : Attribute
{
    public RegisterValidatorAsDependencyAttribute()
    {
        Lifetime = ServiceLifetime.Singleton;
    }

    public RegisterValidatorAsDependencyAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    public ServiceLifetime Lifetime { get; }
}
