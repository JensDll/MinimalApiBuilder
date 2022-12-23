using MinimalApiBuilder;
using Sample.WebApi.Extensions;
using Sample.WebApi.Features.Validation.Async;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogger();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMinimalApiBuilderEndpointsGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var validation = app.MapGroup("/validation").WithTags("Validation");
validation.MapPost<ValidationAsync>("/async");

app.Run();
