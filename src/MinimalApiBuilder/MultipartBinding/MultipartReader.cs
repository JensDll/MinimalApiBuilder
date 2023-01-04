using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalApiBuilder.Entities;

namespace MinimalApiBuilder;

public class MultipartReader : Microsoft.AspNetCore.WebUtilities.MultipartReader
{
    private readonly HttpContext _context;
    private readonly FormOptions _formOptions;

    private MultipartReader(HttpContext context) : base(context.GetBoundary(), context.Request.Body)
    {
        if (!context.IsMultipart())
        {
            throw new MultipartBindingException("Content-Type must be multipart/form-data");
        }

        IOptions<FormOptions> formOptions = context.RequestServices.GetRequiredService<IOptions<FormOptions>>();

        _context = context;
        _formOptions = formOptions.Value;
    }

    public static MultipartReader? Create(HttpContext context)
    {
        try
        {
            return new MultipartReader(context);
        }
        catch (MultipartBindingException e)
        {
            ILoggerFactory loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger<MultipartReader>();
            logger.LogWarning("Failed to create MultipartReader '{Message}'", e.Message);
            return null;
        }
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
