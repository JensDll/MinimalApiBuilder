using System;
using System.IO;
using System.Threading.Tasks;
using Fixture.TestApi.Features.Multipart;
using Fixture.TestApi.Features.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MinimalApiBuilder;
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

Configure(app.MapGet("/products", ProductsEndpoint.Handle));

app.Run();

internal partial class ProductsEndpoint : MinimalApiBuilderEndpoint
{
    public static string Handle(PagingData pageData)
    {
        return pageData.ToString();
    }
}

internal record PagingData(string? SortBy, SortDirection SortDirection, int CurrentPage)
{
    private const string SortByKey = "sortby";
    private const string SortDirectionKey = "sortdir";
    private const string PageKey = "page";

    public static ValueTask<PagingData?> BindAsync(HttpContext httpContext)
    {
        ProductsEndpoint endpoint = httpContext.RequestServices.GetRequiredService<ProductsEndpoint>();

        SortDirection sortDirection = default;
        int page = default;

        if (httpContext.Request.Query.TryGetValue(SortDirectionKey, out StringValues sortDirectionValues))
        {
            if (!Enum.TryParse(sortDirectionValues, ignoreCase: true, out sortDirection))
            {
                endpoint.AddValidationError(SortDirectionKey,
                    "Invalid sort direction. Valid values are 'default', 'asc', or 'desc'.");
            }
        }
        else
        {
            endpoint.AddValidationError(SortDirectionKey, "Missing sort direction.");
        }

        if (httpContext.Request.Query.TryGetValue(PageKey, out StringValues pageValues))
        {
            if (!int.TryParse(pageValues, out page))
            {
                endpoint.AddValidationError(PageKey, "Invalid page number.");
            }
        }
        else
        {
            endpoint.AddValidationError(PageKey, "Missing page number.");
        }

        if (endpoint.HasValidationError)
        {
            return ValueTask.FromResult<PagingData?>(null);
        }

        PagingData result = new(httpContext.Request.Query[SortByKey], sortDirection, page);

        return ValueTask.FromResult<PagingData?>(result);
    }
}

internal enum SortDirection
{
    Default,
    Asc,
    Desc
}
