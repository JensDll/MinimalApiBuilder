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
using MinimalApiBuilder.Generator;
using MinimalApiBuilder.Middleware;
using static MinimalApiBuilder.Generator.ConfigureEndpoints;

#if NET8_0_OR_GREATER
WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
#else
WebApplicationBuilder builder = WebApplication.CreateBuilder();
#endif

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

#if NET8_0_OR_GREATER
builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.TypeInfoResolverChain.RemoveAt(1); // Remove DefaultJsonTypeInfoResolver
    options.SerializerOptions.TypeInfoResolverChain.Add(MultipartJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(ValidationJsonContext.Default);
});
#endif


DefaultFilesOptions defaultFilesOptions = new()
{
    DefaultFileNames = ["index.html"]
};

CompressedStaticFileOptions staticFileOptions = new()
{
    OnPrepareResponse = context =>
    {
        IHeaderDictionary headers = context.Context.Response.Headers;

        if (context.File.Name == "index.html")
        {
            headers.CacheControl = "private,no-store,no-cache,max-age=0,must-revalidate";
            headers.XContentTypeOptions = "nosniff";
            headers.XXSSProtection = "0";
            return;
        }

        headers.CacheControl = "public,max-age=31536000,immutable";
        headers.XContentTypeOptions = "nosniff";
    }
};

builder.Services.AddCompressedStaticFileMiddleware(staticFileOptions);

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

app.UseStatusCodePages();

RouteGroupBuilder validation = app.MapGroup("/validation").WithTags("Validation");
Configure(
    validation.MapPost("/sync/single", SyncSingleValidationEndpoint.Handle),
    validation.MapPatch("/sync/multiple", SyncMultipleValidationEndpoint.Handle),
    validation.MapPost("/async/single", AsyncSingleValidationEndpoint.Handle),
    validation.MapPatch("/async/multiple", AsyncMultipleValidationEndpoint.Handle),
    validation.MapPut("/combination", CombinedValidationEndpoint.Handle));

RouteGroupBuilder multipart = app.MapGroup("/multipart").WithTags("Multipart");
Configure(multipart.MapPost("/zipstream", ZipStreamEndpoint.Handle));
Configure(multipart.MapPost("/bufferedfiles", BufferedFilesEndpoint.Handle));

app.Run();
