using Fixture.TestApi.Extensions;
using Fixture.TestApi.Features.Validation;
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

app.MapValidationFeatures();

app.Run();
