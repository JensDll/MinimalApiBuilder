﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Validation;

internal partial class SyncSingleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(
        [FromServices] SyncSingleValidationEndpoint endpoint,
        SyncValidationRequest request)
    {
        return TypedResults.Ok();
    }
}

internal partial class SyncMultipleValidationEndpoint : MinimalApiBuilderEndpoint
{
    private static IResult Handle(
        [FromServices] SyncMultipleValidationEndpoint endpoint,
        [AsParameters] SyncValidationParameters parameters,
        SyncValidationRequest request)
    {
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
            .Must(static value => bool.TryParse(value, out bool result) && result);
    }
}

internal class SyncValidationParametersValidator : AbstractValidator<SyncValidationParameters>
{
    public SyncValidationParametersValidator()
    {
        RuleFor(static parameters => parameters.Value)
            .Must(static value => value % 2 == 0);
    }
}
