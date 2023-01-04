using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterValidatorAttribute : Attribute
{
    public RegisterValidatorAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    public ServiceLifetime Lifetime { get; }
}
