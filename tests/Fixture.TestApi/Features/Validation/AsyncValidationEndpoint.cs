using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation;

internal partial class AsyncSingleValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static async Task<IResult> Handle(
        [FromServices] AsyncSingleValidationEndpoint endpoint,
        [FromServices] ILogger<AsyncSingleValidationEndpoint> logger,
        AsyncValidationRequest request,
        HttpContext context)
    {
        logger.AsyncValidationRequest(request);
        await Task.CompletedTask;
        MultipartReader _ = new(context, endpoint);
        return TypedResults.Ok(endpoint.ValidationErrors);
    }
}

internal partial class AsyncMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    public static async Task<IResult> Handle(
        [FromServices] ILogger<AsyncMultipleValidationEndpoint> logger,
        [AsParameters] AsyncValidationParameters parameters,
        AsyncValidationRequest request)
    {
        logger.AsyncValidationParameters(parameters);
        logger.AsyncValidationRequest(request);
        await Task.CompletedTask;
        return TypedResults.Ok();
    }
}

internal record struct AsyncValidationParameters(int Value);

internal record AsyncValidationRequest(string Value);

[RegisterValidator(ServiceLifetime.Transient)]
internal class AsyncValidationRequestValidator : AbstractValidator<AsyncValidationRequest>
{
    public AsyncValidationRequestValidator()
    {
        RuleFor(static request => request.Value).MustAsync(static async (value, _) =>
        {
            await Task.CompletedTask;
            return bool.TryParse(value, out bool result) && result;
        }).WithMessage("Property '{PropertyName}' with value '{PropertyValue}' must be true.");
    }
}

internal class AsyncValidationParametersValidator : AbstractValidator<AsyncValidationParameters>
{
    public AsyncValidationParametersValidator()
    {
        RuleFor(static parameters => parameters.Value).MustAsync(static async (value, _) =>
        {
            await Task.CompletedTask;
            return value % 2 == 0;
        }).WithMessage("Parameter '{PropertyName}' with value '{PropertyValue}' must be even.");
    }
}

internal static class AsyncValidationLoggingExtensions
{
    private static readonly Action<ILogger, AsyncValidationParameters, Exception?> s_parameters =
        LoggerMessage.Define<AsyncValidationParameters>(LogLevel.Information,
            new EventId(1, nameof(AsyncValidationRequest)), "Parameters: {Parameters}");

    private static readonly Action<ILogger, AsyncValidationRequest, Exception?> s_request =
        LoggerMessage.Define<AsyncValidationRequest>(LogLevel.Information,
            new EventId(2, nameof(AsyncValidationRequest)), "Request: {Request}");

    public static void AsyncValidationParameters(this ILogger logger, AsyncValidationParameters parameters)
    {
        s_parameters(logger, parameters, null);
    }

    public static void AsyncValidationRequest(this ILogger logger, AsyncValidationRequest request)
    {
        s_request(logger, request, null);
    }
}
