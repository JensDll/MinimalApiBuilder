using Fixture.TestApi.Extensions;
using Fixture.TestApi.Features.Multipart;
using Fixture.TestApi.Features.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApiBuilder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSerilogLogger();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMinimalApiBuilderEndpoints();
builder.Services.AddProblemDetails();

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();

RouteGroupBuilder validation = app.MapGroup("/validation").WithTags("Validation");
validation.MapPost<SyncSingleValidationEndpoint>("/sync/single");
validation.MapPatch<SyncMultipleValidationEndpoint>("/sync/multiple");
validation.MapPost<AsyncSingleValidationEndpoint>("/async/single");
validation.MapPatch<AsyncMultipleValidationEndpoint>("/async/multiple");
validation.MapPut<CombinedValidationEndpoint>("/combination");
RouteGroupBuilder multipart = app.MapGroup("/multipart").WithTags("Multipart");
multipart.MapPost<ZipStreamEndpoint>("/zipstream");
multipart.MapPost<BufferedFilesEndpoint>("/bufferedfiles");

app.Run();
