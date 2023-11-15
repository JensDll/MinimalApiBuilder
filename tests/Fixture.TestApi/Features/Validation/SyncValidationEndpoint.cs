using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation;

internal partial class SyncSingleValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static IResult Handle(
        SyncValidationRequest request,
        Serilog.ILogger logger)
    {
        logger.Information("Request: {Request}", request);
        return TypedResults.Ok();
    }
}

internal partial class SyncMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static IResult Handle(
        [FromServices] SyncMultipleValidationEndpoint endpoint,
        [AsParameters] SyncValidationParameters parameters,
        SyncValidationRequest request,
        Serilog.ILogger logger)
    {
        logger.Information("Endpoint: {Endpoint}", nameof(SyncMultipleValidationEndpoint));
        logger.Information("Parameters: {Parameters}", parameters);
        logger.Information("Request: {Request}", request);
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
