using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace MinimalApiBuilder.Middleware;

internal sealed class CompressedStaticFileOptionsValidator : AbstractValidator<CompressedStaticFileOptions>
{
    public CompressedStaticFileOptionsValidator(ILogger<CompressedStaticFileMiddleware> logger,
        IWebHostEnvironment environment)
    {
        RuleFor(static options => options.FileProvider)
            .Must(fileProvider =>
            {
                if (fileProvider is not NullFileProvider)
                {
                    return true;
                }

                logger.WebRootPathNotFound(Path.GetFullPath(Path.Combine(environment.ContentRootPath,
                    environment.WebRootPath ?? "wwwroot")));

                return false;
            })
            .WithSeverity(Severity.Warning);
    }
}
