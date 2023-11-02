using System.Reflection;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;
using Serilog;

namespace Fixture.TestApi.Features.CustomBinding.BindAsync;

public struct Request
{
    public required string Value { get; init; }

    public static async ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo info)
    {
        await Task.CompletedTask;
        BindAsyncEndpoint endpoint = context.RequestServices.GetRequiredService<BindAsyncEndpoint>();
        endpoint.Logger.Information("Binding request {Name} (BindAsync) ", info.Name);
        endpoint.AddValidationError("BindAsyncFailed");
        return null;
    }
}

internal struct Foo
{
    public static bool TryParse(string value, out Foo result)
    {
        result = default;
        return false;
    }
}

public partial class BindAsyncEndpoint : MinimalApiBuilderEndpoint
{
    public BindAsyncEndpoint(ILogger logger)
    {
        Logger = logger;
    }

    public ILogger Logger { get; }

    private static IResult Handle([FromServices] BindAsyncEndpoint endpoint, Request foo)
    {
        return TypedResults.Ok();
    }

    public static void Configure(RouteHandlerBuilder builder)
    { }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(static request => request.Value).MinimumLength(2);
    }
}
