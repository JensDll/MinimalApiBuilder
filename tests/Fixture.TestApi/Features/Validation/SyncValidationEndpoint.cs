using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MinimalApiBuilder;

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

internal static class SyncValidationLoggingExtensions
{
    private static readonly Action<ILogger, SyncValidationParameters, Exception?> s_parameters =
        LoggerMessage.Define<SyncValidationParameters>(LogLevel.Information,
            new EventId(1, nameof(SyncValidationParameters)), "Parameters: {Parameters}");

    private static readonly Action<ILogger, SyncValidationRequest, Exception?> s_request =
        LoggerMessage.Define<SyncValidationRequest>(LogLevel.Information,
            new EventId(2, nameof(SyncValidationRequest)), "Request: {Request}");

    internal static void SyncValidationParameters(this ILogger logger, SyncValidationParameters parameters)
    {
        s_parameters(logger, parameters, null);
    }

    internal static void SyncValidationRequest(this ILogger logger, SyncValidationRequest request)
    {
        s_request(logger, request, null);
    }
}
