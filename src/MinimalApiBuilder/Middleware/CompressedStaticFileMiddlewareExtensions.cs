using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApiBuilder.Middleware;

/// <summary>
/// Extension methods for the <see cref="CompressedStaticFileMiddleware" />.
/// </summary>
public static class CompressedStaticFileMiddlewareExtensions
{
    /// <summary>
    /// Registers the singleton <see cref="CompressedStaticFileMiddleware" /> with specified options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="options">
    /// The <see cref="CompressedStaticFileOptions" /> used to configure the middleware.
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddCompressedStaticFileMiddleware(
        this IServiceCollection services,
        CompressedStaticFileOptions options)
    {
        AddOptions(services, options);
        services.AddSingleton<CompressedStaticFileMiddleware>();
        return services;
    }

    /// <summary>
    /// Registers the singleton <see cref="CompressedStaticFileMiddleware" /> with default options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <returns></returns>
    public static IServiceCollection AddCompressedStaticFileMiddleware(this IServiceCollection services)
    {
        AddOptions(services, new CompressedStaticFileOptions());
        services.AddSingleton<CompressedStaticFileMiddleware>();
        return services;
    }

    /// <summary>
    /// Enables serving static pre-compressed files based on the
    /// <a href="https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3">Accept-Encoding</a> header field.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder" />.</param>
    public static void UseCompressedStaticFiles(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<CompressedStaticFileMiddleware>();
    }

    private static void AddOptions(IServiceCollection services, CompressedStaticFileOptions options)
    {
        services.AddOptions<CompressedStaticFileOptions>()
            .PostConfigure<IWebHostEnvironment>((currentOptions, environment) =>
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
                currentOptions.ContentEncoding = options.ContentEncoding;
            })
            .FluentValidation<CompressedStaticFileOptions, CompressedStaticFileOptionsValidator>()
            .ValidateOnStart();
    }
}
