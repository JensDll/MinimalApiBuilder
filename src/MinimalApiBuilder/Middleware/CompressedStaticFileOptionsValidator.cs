using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

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

        RuleFor(static options => options.ContentEncoding)
            .Must(dictionary => dictionary.Values.All(static value => value.Order >= 0))
            .WithMessage("{PropertyName} order values must not be negative")
            .Must(dictionary => dictionary.Comparer.Equals(StringSegmentComparer.OrdinalIgnoreCase))
            .WithMessage("{PropertyName} keys must use case-insensitive ordinal comparison");
    }
}
