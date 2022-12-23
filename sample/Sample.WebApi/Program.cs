using MinimalApiBuilder;
using Sample.WebApi.Extensions;
using Sample.WebApi.Features.Validation.Async;
using Sample.WebApi.Features.Validation.Sync;

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
validation.MapPost<ValidationAsync>("/async");
validation.MapPost<ValidationSync>("/sync");

app.Run();
