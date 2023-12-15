using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MinimalApiBuilder;

/// <summary>
/// Extension methods for <see cref="OptionsBuilder{TOptions}" />.
/// </summary>
public static class OptionsBuilderExtensions
{
    /// <summary>
    /// Validates options using FluentValidation.
    /// </summary>
    /// <param name="builder">The current <see cref="OptionsBuilder{TOptions}" />.</param>
    /// <typeparam name="TOptions">The options type to validate.</typeparam>
    /// <typeparam name="TValidator">The <see cref="AbstractValidator{T}" /> of the options type.</typeparam>
    /// <returns></returns>
    public static OptionsBuilder<TOptions> FluentValidation
        <TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TValidator>
        (this OptionsBuilder<TOptions> builder)
        where TOptions : class
        where TValidator : AbstractValidator<TOptions>
    {
        builder.Services.TryAddSingleton<IValidator<TOptions>, TValidator>();
        builder.Services.AddSingleton<IValidateOptions<TOptions>>(static serviceProvider =>
        {
            var validator = serviceProvider.GetRequiredService<IValidator<TOptions>>();
            return new FluentValidationValidateOptions<TOptions>(validator);
        });

        return builder;
    }
}
