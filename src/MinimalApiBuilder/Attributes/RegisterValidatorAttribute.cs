using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder;

/// <summary>
/// Register <see cref="AbstractValidator{T}" /> with dependency injection.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterValidatorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="RegisterValidatorAttribute" />.
    /// </summary>
    /// <param name="lifetime">
    /// The service lifetime with which the <see cref="AbstractValidator{T}" /> will be added to the <see cref="IServiceCollection" />
    /// </param>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
    [SuppressMessage("Design", "CA1019:Define accessors for attribute arguments")]
    public RegisterValidatorAttribute(ServiceLifetime lifetime)
    { }
}
