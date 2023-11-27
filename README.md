# MinimalApiBuilder

[![nuget](https://badgen.net/nuget/v/MinimalApiBuilder)](https://www.nuget.org/packages/MinimalApiBuilder)

Reflectionless, source-generated, thin abstraction layer over
the [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-gb/aspnet/core/fundamentals/minimal-apis/overview)
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
and have a `static` `Handle` or `HandleAsync` method.
The endpoint is mapped through the typical `IEndpointRouteBuilder` `Map<Verb>` extension methods:

```csharp
app.MapGet("/hello", BasicEndpoint.Handle);
```

This library depends on [`FluentValidation >= 11`](https://github.com/FluentValidation/FluentValidation). An endpoint can have a validated request object:

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

The incremental generator will generate code to validate the request object before the handler is called and return a
[`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem)
validation error result if the validation fails. To wire up the validation filters and to support the
[Request Delegate Generator](https://learn.microsoft.com/en-gb/aspnet/core/fundamentals/aot/request-delegate-generator/rdg),
the `Map` methods need to be wrapped by the `ConfigureEndpoints.Configure` helper, which expects a comma-separated list of
[`RouteHandlerBuilder`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.routehandlerbuilder):

```csharp
using static MinimalApiBuilder.ConfigureEndpoints;

Configure(app.MapGet("/hello/{name}", BasicRequestEndpoint.Handle));
```

Validation in [custom binding](https://learn.microsoft.com/en-gb/aspnet/core/fundamentals/minimal-apis/parameter-binding#custom-binding)
scenarios is also supported.

In `Program.cs` the below

```csharp
builder.Services.AddMinimalApiBuilderEndpoints();
```

needs to be added to register the necessary types with dependency injection.

## Configuration

Users can add configuration through entries in `.editorconfig` or with MSBuild properties.
The following options are available:

### `minimalapibuilder_assign_name_to_endpoint` (`true` | `false`)

If `true`, the generator will add a unique `public const string Name` field
to the endpoint classes and call
the [`WithName`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.routingendpointconventionbuilderextensions.withname)
extension method when mapping them.

```.editorconfig
minimalapibuilder_assign_name_to_endpoint = true
```

```xml
<PropertyGroup>
  <minimalapibuilder_assign_name_to_endpoint>true</minimalapibuilder_assign_name_to_endpoint>
</PropertyGroup>
```

### `minimalapibuilder_validation_problem_type` (`string`)

The type of the [`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem) validation error result.
The configuration below is the default.

```.editorconfig
minimalapibuilder_validation_problem_type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
```

```xml
<PropertyGroup>
  <minimalapibuilder_validation_problem_type>https://tools.ietf.org/html/rfc9110#section-15.5.1</minimalapibuilder_validation_problem_type>
</PropertyGroup>
```

### `minimalapibuilder_validation_problem_title` (`string`)

The title of the [`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem) validation error result.
The configuration below is the default.

```.editorconfig
minimalapibuilder_validation_problem_title = "One or more validation errors occurred."
```

```xml
<PropertyGroup>
  <minimalapibuilder_validation_problem_title>One or more validation errors occurred.</minimalapibuilder_validation_problem_title>
</PropertyGroup>
```

### `minimalapibuilder_model_binding_problem_type` (`string`)

The type of the [`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem) model binding error result.
The configuration below is the default.

```.editorconfig
minimalapibuilder_model_binding_problem_type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
```

```xml
<PropertyGroup>
  <minimalapibuilder_model_binding_problem_type>https://tools.ietf.org/html/rfc9110#section-15.5.1</minimalapibuilder_model_binding_problem_type>
</PropertyGroup>
```

### `minimalapibuilder_model_binding_problem_title` (`string`)

The title of the [`ValidationProblem`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.validationproblem) model binding error result.
The configuration below is the default.

```.editorconfig
minimalapibuilder_model_binding_problem_title = "One or more model binding errors occurred."
```

```xml
<PropertyGroup>
  <minimalapibuilder_model_binding_problem_title>One or more model binding errors occurred.</minimalapibuilder_model_binding_problem_title>
</PropertyGroup>
```
