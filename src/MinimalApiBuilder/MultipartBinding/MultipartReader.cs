using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MinimalApiBuilder.Entities;

namespace MinimalApiBuilder;

public class MultipartReader : Microsoft.AspNetCore.WebUtilities.MultipartReader
{
    private readonly HttpContext _context;
    private readonly FormOptions _formOptions;

    /// <summary>
    /// Initializes a new instance of <see cref="MultipartReader" />.
    /// </summary>
    /// <param name="context">The current HTTP request context.</param>
    /// <exception cref="MultipartBindingException">
    /// Thrown if the request is not a multipart request.
    /// </exception>
    /// <seealso cref="Microsoft.AspNetCore.WebUtilities.MultipartReader" />
    public MultipartReader(HttpContext context) : base(context.GetBoundary(), context.Request.Body)
    {
        if (!context.IsMultipart())
        {
            throw new MultipartBindingException("Content-Type must be multipart/form-data");
        }

        IOptions<FormOptions> formOptions = context.RequestServices.GetRequiredService<IOptions<FormOptions>>();

        _context = context;
        _formOptions = formOptions.Value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="MultipartReader" />. Multipart request errors are added to
    /// <see cref="MinimalApiBuilderEndpoint" />.<see cref="MinimalApiBuilderEndpoint.ValidationErrors" />
    /// without throwing an exception.
    /// </summary>
    /// <param name="context">The current HTTP request context.</param>
    /// <param name="endpoint">The current endpoint handling the request.</param>
    /// <seealso cref="Microsoft.AspNetCore.WebUtilities.MultipartReader" />
    public MultipartReader(HttpContext context, MinimalApiBuilderEndpoint endpoint)
        : base(context.GetBoundary(endpoint), context.Request.Body)
    {
        if (!context.IsMultipart())
        {
            endpoint.AddValidationError("Content-Type must be multipart/form-data");
        }

        IOptions<FormOptions> formOptions = context.RequestServices.GetRequiredService<IOptions<FormOptions>>();

        _context = context;
        _formOptions = formOptions.Value;
    }

    public new async Task<NextSection?> ReadNextSectionAsync(CancellationToken cancellationToken = default)
    {
        MultipartSection? section = await base.ReadNextSectionAsync(cancellationToken);
        return section is null ? null : new NextSection(section);
    }

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

        return new NextSection(section);
    }

    public async Task<IReadOnlyList<FormFile>?> ReadFilesBufferedAsync(CancellationToken cancellationToken = default)
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
