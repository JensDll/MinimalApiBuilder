using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder.Generator;

/// <summary>
/// Attribute used by the minimal API builder generator
/// to select the <see cref="ServiceLifetime" /> of the <see cref="AbstractValidator{T}" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterValidatorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="RegisterValidatorAttribute" />.
    /// </summary>
    /// <param name="lifetime">
    /// The service lifetime with which the <see cref="AbstractValidator{T}" />
    /// is added to the <see cref="IServiceCollection" />; defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
    [SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
    // ReSharper disable once UnusedParameter.Local
    public RegisterValidatorAttribute(ServiceLifetime lifetime)
    { }
}
