using System.Threading.Tasks;
using Fixture.TestApi.Extensions;
using Fixture.TestApi.Features.CustomBinding;
using Fixture.TestApi.Features.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApiBuilder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSerilogLogger();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMinimalApiBuilderEndpoints();

builder.Services.Configure<RouteHandlerOptions>(static options =>
{
    options.ThrowOnBadRequest = false;
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();

app.MapValidationFeatures();
app.MapCustomBindingFeatures();

app.MapPost("/foo", static (Foo r) => $"Hello {r.Value}!")
    .AddEndpointFilter(static (ic, next) =>
    {
        return next(ic);
    });

app.Run();

internal class Foo
{
    public required string Value { get; init; }

    public static async ValueTask<Foo> BindAsync(HttpContext context)
    {
        await Task.CompletedTask;
        return null!;
    }
}
