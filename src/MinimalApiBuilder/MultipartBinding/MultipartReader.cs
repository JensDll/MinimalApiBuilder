using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MinimalApiBuilder.Entities;

namespace MinimalApiBuilder;

/// <summary>
/// A multipart/form-data reader using <see cref="MinimalApiBuilderEndpoint" /> to hold validation errors.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.WebUtilities.MultipartReader" />
public class MultipartReader : Microsoft.AspNetCore.WebUtilities.MultipartReader
{
    private readonly HttpContext _context;
    private readonly FormOptions _formOptions;
    private readonly MinimalApiBuilderEndpoint _endpoint;

    /// <summary>
    /// Initializes a new instance of <see cref="MultipartReader" />. Multipart request errors are added to the
    /// <see cref="MinimalApiBuilderEndpoint.ValidationErrors" /> without throwing an exception.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext" />.</param>
    /// <param name="endpoint">The current <see cref="MinimalApiBuilderEndpoint" /> handling the request.</param>
    public MultipartReader(HttpContext context, MinimalApiBuilderEndpoint endpoint)
        : base(context.GetBoundary(endpoint), context.Request.Body)
    {
        if (!context.IsMultipart())
        {
            endpoint.AddValidationError("multipart", "Content-Type must be multipart/form-data");
        }

        IOptions<FormOptions> formOptions = context.RequestServices.GetRequiredService<IOptions<FormOptions>>();

        _context = context;
        _formOptions = formOptions.Value;
        _endpoint = endpoint;
    }

    /// <summary>
    /// Reads the next <see cref="NextSection" /> from the underlying
    /// <see cref="Microsoft.AspNetCore.WebUtilities.MultipartReader" />.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to cancel the operation.</param>
    /// <returns></returns>
    public new async Task<NextSection?> ReadNextSectionAsync(CancellationToken cancellationToken = default)
    {
        MultipartSection? section = await base.ReadNextSectionAsync(cancellationToken);
        return section is null ? null : new NextSection(section, _endpoint);
    }

    /// <summary>
    /// Reads the next <see cref="NextSection" /> from the underlying
    /// <see cref="Microsoft.AspNetCore.WebUtilities.MultipartReader" />
    /// using a <see cref="FileBufferingReadStream" /> as the body.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to cancel the operation.</param>
    /// <returns></returns>
    public async Task<NextSection?> ReadNextSectionBufferedAsync(CancellationToken cancellationToken = default)
    {
        MultipartSection? section = await base.ReadNextSectionAsync(cancellationToken);

        if (section is null)
        {
            return null;
        }

        FileBufferingReadStream fileStream = new(section.Body, _formOptions.MemoryBufferThreshold,
            _formOptions.MultipartBodyLengthLimit, TempDirectory.Path);
        section.Body = fileStream;
        _context.Response.RegisterForDisposeAsync(fileStream);

        return new NextSection(section, _endpoint);
    }

    /// <summary>
    /// Continually reads the next <see cref="NextSection" /> from the underlying
    /// <see cref="Microsoft.AspNetCore.WebUtilities.MultipartReader" />
    /// using <see cref="ReadNextSectionBufferedAsync" />.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> used to cancel the operation.</param>
    /// <returns></returns>
    public async Task<IReadOnlyList<IFormFile>?> ReadFilesBufferedAsync(CancellationToken cancellationToken = default)
    {
        List<FormFile> files = new();

        while (await ReadNextSectionBufferedAsync(cancellationToken) is { } nextSection)
        {
            FileMultipartSection? fileSection = nextSection.AsFileSection();

            if (fileSection is null)
            {
                return null;
            }

            await fileSection.Section.Body.DrainAsync(cancellationToken);
            fileSection.Section.Body.Position = 0;

            FormFile file = new(fileSection.Section.Body, fileSection.Section.BaseStreamOffset ?? 0,
                fileSection.Section.Body.Length, fileSection.Name, fileSection.FileName);

            files.Add(file);
        }

        return files.Count != 0 ? files : null;
    }
}
