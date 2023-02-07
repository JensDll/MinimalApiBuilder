using Fixture.TestApi.Extensions;
using MinimalApiBuilder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogger();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMinimalApiBuilderEndpoints();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

RouteGroupBuilder validation = app.MapGroup("/validation").WithTags("Validation");
validation.MapPost<Fixture.TestApi.Features.Validation.Synchronous.EndpointSingle>("/sync/single");
validation.MapPatch<Fixture.TestApi.Features.Validation.Synchronous.EndpointMultiple>("/sync/multiple");
validation.MapPost<Fixture.TestApi.Features.Validation.Asynchronous.EndpointSingle>("/async/single");
validation.MapPatch<Fixture.TestApi.Features.Validation.Asynchronous.EndpointMultiple>("/async/multiple");
validation.MapPut<Fixture.TestApi.Features.Validation.Combination.Endpoint>("/combination");

app.Run();
