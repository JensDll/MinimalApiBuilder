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
    /// Enables serving static files, choosing pre-compressed files based on the
    /// <a href="https://www.rfc-editor.org/rfc/rfc9110.html#section-12.5.3">Accept-Encoding</a> header field.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder" />.</param>
    public static void UseCompressedStaticFiles(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<CompressedStaticFileMiddleware>();
    }

    private static void AddOptions(IServiceCollection services, CompressedStaticFileOptions o)
    {
        services.AddOptions<CompressedStaticFileOptions>()
            .PostConfigure<IWebHostEnvironment>((options, environment) =>
            {
                // https://github.com/dotnet/aspnetcore/blob/v8.0.0/src/Middleware/StaticFiles/src/Infrastructure/SharedOptionsBase.cs
                options.RequestPath = o.RequestPath;
                options.FileProvider = o.FileProvider ?? environment.WebRootFileProvider;
                options.RedirectToAppendTrailingSlash = o.RedirectToAppendTrailingSlash;

                // https://github.com/dotnet/aspnetcore/blob/v8.0.0/src/Middleware/StaticFiles/src/StaticFileOptions.cs
                options.ContentTypeProvider = o.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
                options.DefaultContentType = o.DefaultContentType;
                options.ServeUnknownFileTypes = o.ServeUnknownFileTypes;
                options.HttpsCompression = o.HttpsCompression;
                options.OnPrepareResponse = o.OnPrepareResponse;
                options.OnPrepareResponseAsync = o.OnPrepareResponseAsync;

                // CompressedStaticFileOptions
                options.ContentCoding = o.ContentCoding;
                options.Initialize();
            })
            .FluentValidation<CompressedStaticFileOptions, CompressedStaticFileOptionsValidator>()
            .ValidateOnStart();
    }

    internal static bool IsNotAllowed(this IdentityAllowedFlags flags)
    {
        return (flags & (IdentityAllowedFlags.Allowed | IdentityAllowedFlags.NotAllowed)) ==
               IdentityAllowedFlags.NotAllowed;
    }
}
