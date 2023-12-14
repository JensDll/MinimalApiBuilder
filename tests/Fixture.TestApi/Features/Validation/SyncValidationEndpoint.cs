using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MinimalApiBuilder.Generator;

namespace Fixture.TestApi.Features.Validation;

internal partial class SyncSingleValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static IResult Handle(
        [FromServices] ILogger<SyncSingleValidationEndpoint> logger,
        SyncValidationRequest request)
    {
        logger.SyncValidationRequest(request);
        return TypedResults.Ok();
    }
}

internal partial class SyncMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static IResult Handle(
        [FromServices] ILogger<SyncMultipleValidationEndpoint> logger,
        [AsParameters] SyncValidationParameters parameters,
        SyncValidationRequest request)
    {
        logger.SyncValidationParameters(parameters);
        logger.SyncValidationRequest(request);
        return TypedResults.Ok();
    }
}

internal record struct SyncValidationParameters(int Value);

internal record SyncValidationRequest(string Value);

internal class SyncValidationRequestValidator : AbstractValidator<SyncValidationRequest>
{
    public SyncValidationRequestValidator()
    {
        RuleFor(static request => request.Value)
            .Must(static value => bool.TryParse(value, out bool result) && result)
            .WithMessage("Property '{PropertyName}' with value '{PropertyValue}' must be true.");
    }
}

internal class SyncValidationParametersValidator : AbstractValidator<SyncValidationParameters>
{
    public SyncValidationParametersValidator()
    {
        RuleFor(static parameters => parameters.Value)
            .Must(static value => value % 2 == 0)
            .WithMessage("Parameter '{PropertyName}' with value '{PropertyValue}' must be even.");
    }
}

internal static partial class SyncValidationLoggingExtensions
{
    [LoggerMessage(0, LogLevel.Information, $"{nameof(SyncValidationParameters)}: {{Parameters}}")]
    internal static partial void SyncValidationParameters(this ILogger logger, SyncValidationParameters parameters);

    [LoggerMessage(1, LogLevel.Information, $"{nameof(SyncValidationRequest)}: {{Request}}")]
    internal static partial void SyncValidationRequest(this ILogger logger, SyncValidationRequest request);
}
