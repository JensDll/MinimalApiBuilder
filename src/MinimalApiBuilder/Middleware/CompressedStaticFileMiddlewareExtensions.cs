using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace MinimalApiBuilder.Middleware;

/// <summary>
/// Extension methods for the <see cref="CompressedStaticFileMiddleware" />.
/// </summary>
public static class CompressedStaticFileMiddlewareExtensions
{
    /// <summary>
    /// Registers the scoped <see cref="CompressedStaticFileMiddleware" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="options">
    /// The <see cref="CompressedStaticFileOptions" /> used to configure
    /// the <see cref="CompressedStaticFileMiddleware" /> behavior.
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddCompressedStaticFileMiddleware(
        this IServiceCollection services,
        CompressedStaticFileOptions options)
    {
        AddOptions(services, options);
        services.AddScoped<CompressedStaticFileMiddleware>();
        return services;
    }

    /// <summary>
    /// Registers the scoped <see cref="CompressedStaticFileMiddleware" /> with default options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <returns></returns>
    public static IServiceCollection AddCompressedStaticFileMiddleware(this IServiceCollection services)
    {
        AddOptions(services, new CompressedStaticFileOptions());
        services.AddScoped<CompressedStaticFileMiddleware>();
        return services;
    }

    /// <summary>
    /// Enables serving static and static compressed files based on the Accept-Encoding header field.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder" />.</param>
    /// <remarks>
    /// A drop-in replacement for the <see cref="StaticFileMiddleware" />
    /// but uses factory-based middleware over convention-based middleware.
    /// </remarks>
    public static void UseCompressedStaticFiles(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<CompressedStaticFileMiddleware>();
    }

    private static void AddOptions(IServiceCollection services, CompressedStaticFileOptions options)
    {
        services.AddOptions<CompressedStaticFileOptions>()
            .PostConfigure<IWebHostEnvironment, ILogger<CompressedStaticFileMiddleware>>(
                (currentOptions, environment, logger) =>
                {
                    // https://github.com/dotnet/aspnetcore/blob/v8.0.0/src/Middleware/StaticFiles/src/Infrastructure/SharedOptionsBase.cs
                    currentOptions.RequestPath = options.RequestPath;
                    currentOptions.FileProvider = options.FileProvider ?? environment.WebRootFileProvider;
                    options.RedirectToAppendTrailingSlash = options.RedirectToAppendTrailingSlash;
                    // https://github.com/dotnet/aspnetcore/blob/v8.0.0/src/Middleware/StaticFiles/src/StaticFileOptions.cs
                    currentOptions.ContentTypeProvider =
                        options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
                    currentOptions.DefaultContentType = options.DefaultContentType;
                    currentOptions.ServeUnknownFileTypes = options.ServeUnknownFileTypes;
                    currentOptions.HttpsCompression = options.HttpsCompression;
                    currentOptions.OnPrepareResponse = options.OnPrepareResponse;
#if NET8_0_OR_GREATER
                    currentOptions.OnPrepareResponseAsync = options.OnPrepareResponseAsync;
#endif
                    // CompressedStaticFileOptions
                    currentOptions.ContentEncodingOrder = options.ContentEncodingOrder;

                    if (currentOptions.FileProvider is NullFileProvider)
                    {
                        logger.WebRootPathNotFound(Path.GetFullPath(
                            Path.Combine(environment.ContentRootPath, environment.WebRootPath ?? "wwwroot")));
                    }
                });
    }
}
