using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features;

public class BasicRequest
{
    public required string Value { get; init; }

    public static async ValueTask<BasicRequest?> BindAsync(HttpContext context)
    {
        CancellationToken cancellationToken = context.RequestAborted;
        BasicEndpoint endpoint = context.RequestServices.GetRequiredService<BasicEndpoint>();
        endpoint.AddValidationError("BindAsync error");
        await Task.Delay(0, cancellationToken);
        return null;
        // return new BasicRequest
        // {
        //     Value = "Foo"
        // };
    }
}

public record struct BasicParameters(int? PageNumber);

public partial class BasicEndpoint : MinimalApiBuilderEndpoint
{
    private static string Handle([FromServices] BasicEndpoint endpoint, BasicRequest? request,
        [AsParameters] BasicParameters parameters)
    {
        return $"({request?.Value}, {parameters.PageNumber}) Errors = {endpoint.ValidationErrors.Count}";
    }
}

public class BasicRequestValidator : AbstractValidator<BasicRequest>
{
    public BasicRequestValidator()
    {
        RuleFor(static request => request.Value).MinimumLength(2);
    }
}
