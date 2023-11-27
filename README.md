# MinimalApiBuilder

[![nuget](https://badgen.net/nuget/v/MinimalApiBuilder)](https://www.nuget.org/packages/MinimalApiBuilder)

Reflectionless, source-generated, thin abstraction layer over the
[ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-gb/aspnet/core/fundamentals/minimal-apis/overview)
interface.

## How to Use

Based on the Vertical Slice Architecture with `Feature` folder.
There is one class for every API endpoint. A basic example looks like the following:

```csharp
using Microsoft.AspNetCore.Mvc;
using MinimalApiBuilder;

public partial class BasicEndpoint : MinimalApiBuilderEndpoint
{
    public static string Handle()
    {
        return "Hello, World!";
    }
}
```

The endpoint class must be `partial`, inherit from `MinimalApiBuilderEndpoint`,
and have a `static` `Handle` or `HandleAsync` method. The endpoint is mapped
through the typical `IEndpointRouteBuilder` `Map<Verb>` extension methods:

```csharp
app.MapGet("/hello", BasicEndpoint.Handle);
```

This library depends on [`FluentValidation >= 11`](https://github.com/FluentValidation/FluentValidation).
An endpoint can have a validated request object:

```csharp
public struct BasicRequest
{
    public required string Name { get; init; }
}

public partial class BasicRequestEndpoint : MinimalApiBuilderEndpoint
{
    public static string Handle([AsParameters] BasicRequest request)
    {
        return $"Hello, {request.Name}!";
    }
}

public class BasicRequestValidator : AbstractValidator<BasicRequest>
{
    public BasicRequestValidator()
    {
        RuleFor(static request => request.Name).MinimumLength(2);
    }
}
```

The incremental generator will generate code to validate the request object before
the handler is called and return a [`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem)
validation error result if the validation fails. To wire up the validation filters
and to support the [Request Delegate Generator](https://learn.microsoft.com/en-gb/aspnet/core/fundamentals/aot/request-delegate-generator/rdg),
the `Map` methods need to be wrapped by the `ConfigureEndpoints.Configure` helper,
which expects a comma-separated list of [`RouteHandlerBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.routehandlerbuilder):

```csharp
using static MinimalApiBuilder.ConfigureEndpoints;

Configure(app.MapGet("/hello/{name}", BasicRequestEndpoint.Handle));
```

Validation in [custom binding](https://learn.microsoft.com/en-gb/aspnet/core/fundamentals/minimal-apis/parameter-binding#custom-binding)
scenarios is also supported. For example, adapting the Microsoft
[`BindAsync` sample](https://learn.microsoft.com/en-gb/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-8.0#bindasync):

<details>
<summary>Show example</summary>

```csharp
public record PagingData(string? SortBy, SortDirection SortDirection, int CurrentPage)
{
    private const string SortByKey = "sortby";
    private const string SortDirectionKey = "sortdir";
    private const string PageKey = "page";

    public static ValueTask<PagingData?> BindAsync(HttpContext httpContext)
    {
        ProductsEndpoint endpoint =
            httpContext.RequestServices.GetRequiredService<ProductsEndpoint>();

        SortDirection sortDirection = default;
        int page = default;

        if (httpContext.Request.Query.TryGetValue(SortDirectionKey,
            out StringValues sortDirectionValues))
        {
            if (!Enum.TryParse(sortDirectionValues, ignoreCase: true, out sortDirection))
            {
                endpoint.AddValidationError(SortDirectionKey,
                    "Invalid sort direction. Valid values are 'default', 'asc', or 'desc'.");
            }
        }
        else
        {
            endpoint.AddValidationError(SortDirectionKey, "Missing sort direction.");
        }

        if (httpContext.Request.Query.TryGetValue(PageKey, out StringValues pageValues))
        {
            if (!int.TryParse(pageValues, out page))
            {
                endpoint.AddValidationError(PageKey, "Invalid page number.");
            }
        }
        else
        {
            endpoint.AddValidationError(PageKey, "Missing page number.");
        }

        if (endpoint.HasValidationError)
        {
            return ValueTask.FromResult<PagingData?>(null);
        }

        PagingData result = new(httpContext.Request.Query[SortByKey], sortDirection, page);

        return ValueTask.FromResult<PagingData?>(result);
    }
}

public enum SortDirection
{
    Default,
    Asc,
    Desc
}

public partial class ProductsEndpoint : MinimalApiBuilderEndpoint
{
    public static string Handle(PagingData pageData)
    {
        return pageData.ToString();
    }
}
```

```csharp
Configure(app.MapGet("/products", ProductsEndpoint.Handle));
```

</details>

Unfortunately, [`TryParse`](https://learn.microsoft.com/en-gb/aspnet/core/fundamentals/minimal-apis/parameter-binding#tryparse)
cannot be validated this way as there is no easy way to access the
`IServiceProvider` right now. To not short-circuit execution by
throwing an exception when returning `null` from `BindAsync`,
[`ThrowOnBadRequest`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routehandleroptions.throwonbadrequest)
needs to be disabled:

```csharp
builder.Services.Configure<RouteHandlerOptions>(static options =>
{
    options.ThrowOnBadRequest = false;
});
```

Endpoints and validators need to be registered
with dependency injection. The following method adds them:

```csharp
builder.Services.AddMinimalApiBuilderEndpoints();
```

## Configuration

Users can add configuration through entries in `.editorconfig` or with
[MSBuild properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-properties).
The following options are available,
with configuration snippets showing the default values:

### `minimalapibuilder_assign_name_to_endpoint` (`true` | `false`)

If `true`, the generator will add a unique `public const string Name` field to
the endpoint classes and call the [`WithName`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.routingendpointconventionbuilderextensions.withname)
extension method when mapping them.

```.editorconfig
minimalapibuilder_assign_name_to_endpoint = false
```

```xml
<PropertyGroup>
  <minimalapibuilder_assign_name_to_endpoint>false</minimalapibuilder_assign_name_to_endpoint>
</PropertyGroup>
```

### `minimalapibuilder_validation_problem_type` (`string`)

The [type](https://datatracker.ietf.org/doc/html/rfc7807#section-3.1) of the
[`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem)
validation error result.

```.editorconfig
minimalapibuilder_validation_problem_type = https://tools.ietf.org/html/rfc9110#section-15.5.1
```

```xml
<PropertyGroup>
  <minimalapibuilder_validation_problem_type>https://tools.ietf.org/html/rfc9110#section-15.5.1</minimalapibuilder_validation_problem_type>
</PropertyGroup>
```

### `minimalapibuilder_validation_problem_title` (`string`)

The [title](https://datatracker.ietf.org/doc/html/rfc7807#section-3.1)
of the [`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem)
validation error result.

```.editorconfig
minimalapibuilder_validation_problem_title = One or more validation errors occurred.
```

```xml
<PropertyGroup>
  <minimalapibuilder_validation_problem_title>One or more validation errors occurred.</minimalapibuilder_validation_problem_title>
</PropertyGroup>
```

### `minimalapibuilder_model_binding_problem_type` (`string`)

The [type](https://datatracker.ietf.org/doc/html/rfc7807#section-3.1)
of the [`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem)
model binding error result.

```.editorconfig
minimalapibuilder_model_binding_problem_type = https://tools.ietf.org/html/rfc9110#section-15.5.1
```

```xml
<PropertyGroup>
  <minimalapibuilder_model_binding_problem_type>https://tools.ietf.org/html/rfc9110#section-15.5.1</minimalapibuilder_model_binding_problem_type>
</PropertyGroup>
```

### `minimalapibuilder_model_binding_problem_title` (`string`)

The [title](https://datatracker.ietf.org/doc/html/rfc7807#section-3.1)
of the [`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem)
model binding error result.

```.editorconfig
minimalapibuilder_model_binding_problem_title = One or more model binding errors occurred.
```

```xml
<PropertyGroup>
  <minimalapibuilder_model_binding_problem_title>One or more model binding errors occurred.</minimalapibuilder_model_binding_problem_title>
</PropertyGroup>
```
