using MinimalApiBuilder;
using Sample.WebApi.Extensions;
using Sample.WebApi.Features.Validation.Asynchronous;
using Sample.WebApi.Features.Validation.Combination;
using Sample.WebApi.Features.Validation.Synchronous;

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

validation.MapPost<SyncValidationSingleEndpoint>("/sync/single");
validation.MapPatch<SyncValidationMultipleEndpoint>("/sync/multiple");

validation.MapPost<AsyncValidationSingleEndpoint>("/async/single");
validation.MapPatch<AsyncValidationMultipleEndpoint>("/async/multiple");

validation.MapPut<SyncAsyncValidationEndpoint>("/combination");

app.Run();
