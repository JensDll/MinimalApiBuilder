using System.IO;
using Fixture.TestApi.Features.Multipart;
using Fixture.TestApi.Features.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinimalApiBuilder;
using MinimalApiBuilder.Middleware;
using static MinimalApiBuilder.ConfigureEndpoints;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile(Path.Join("Properties", "appSettings.json"), optional: false, reloadOnChange: false);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMinimalApiBuilderEndpoints();
builder.Services.AddProblemDetails();

builder.Services.Configure<RouteHandlerOptions>(static options =>
{
    options.ThrowOnBadRequest = false;
});

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.TypeInfoResolverChain.RemoveAt(1); // Remove DefaultJsonTypeInfoResolver
    options.SerializerOptions.TypeInfoResolverChain.Add(MultipartJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(ValidationJsonContext.Default);
});

builder.Services.AddCompressedStaticFileMiddleware();

DefaultFilesOptions defaultFilesOptions = new()
{
    DefaultFileNames = ["index.html"]
};

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles(defaultFilesOptions);
app.UseCompressedStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

CompressedStaticFileOptions options = new()
{
    ContentEncodingOrder = null
};

app.UseStatusCodePages();

RouteGroupBuilder validation = app.MapGroup("/validation").WithTags("Validation");
Configure(
    validation.MapPost("/sync/single", SyncSingleValidationEndpoint.Handle),
    validation.MapPatch("/sync/multiple", SyncMultipleValidationEndpoint.Handle),
    validation.MapPost("/async/single", AsyncSingleValidationEndpoint.Handle));
Configure(
    validation.MapPatch("/async/multiple", AsyncMultipleValidationEndpoint.Handle),
    validation.MapPut("/combination", CombinedValidationEndpoint.Handle));

RouteGroupBuilder multipart = app.MapGroup("/multipart").WithTags("Multipart");
Configure(
    multipart.MapPost("/zipstream", ZipStreamEndpoint.Handle),
    multipart.MapPost("/bufferedfiles", BufferedFilesEndpoint.Handle));

app.Run();
