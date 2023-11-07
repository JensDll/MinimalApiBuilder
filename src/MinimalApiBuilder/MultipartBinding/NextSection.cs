using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace MinimalApiBuilder;

/// <summary>
/// Wrapper around <see cref="MultipartSection" />.
/// </summary>
public class NextSection
{
    private readonly MultipartSection _section;
    private readonly MinimalApiBuilderEndpoint _endpoint;

    internal NextSection(MultipartSection section, MinimalApiBuilderEndpoint endpoint)
    {
        _section = section;
        _endpoint = endpoint;
    }

    /// <summary>
    /// Converts itself to a form section.
    /// </summary>
    /// <returns>The converted <see cref="FormMultipartSection" />.</returns>
    public FormMultipartSection? AsFormSection()
    {
        if (_section.IsFormData(out ContentDispositionHeaderValue? contentDisposition))
        {
            return new FormMultipartSection(_section, contentDisposition);
        }

        _endpoint.AddValidationError($"Multipart section '{contentDisposition?.Name}' is not form data");
        return null;
    }

    /// <summary>
    /// Converts itself to a form section and validates its name.
    /// </summary>
    /// <param name="sectionName">The required form section name.</param>
    /// <returns>The converted <see cref="FormMultipartSection" />.</returns>
    public FormMultipartSection? AsFormSection(string sectionName)
    {
        FormMultipartSection? section = AsFormSection();

        if (section is null)
        {
            return null;
        }

        if (section.Name == sectionName)
        {
            return section;
        }

        _endpoint.AddValidationError($"Multipart section '{section.Name}' does not match '{sectionName}'");
        return null;
    }

    /// <summary>
    /// Converts itself to a file section.
    /// </summary>
    /// <returns>The converted <see cref="FileMultipartSection" />.</returns>
    public FileMultipartSection? AsFileSection()
    {
        if (_section.IsFile(out ContentDispositionHeaderValue? contentDisposition))
        {
            return new FileMultipartSection(_section, contentDisposition);
        }

        _endpoint.AddValidationError($"Multipart section '{contentDisposition?.Name}' is not a file");
        return null;
    }

    /// <summary>
    /// Converts itself to a file section and validates its name.
    /// </summary>
    /// <param name="sectionName">The required file section name.</param>
    /// <returns>The converted <see cref="FileMultipartSection" />.</returns>
    public FileMultipartSection? AsFileSection(string sectionName)
    {
        FileMultipartSection? section = AsFileSection();

        if (section is null)
        {
            return null;
        }

        if (section.Name == sectionName)
        {
            return section;
        }

        _endpoint.AddValidationError($"Multipart section '{section.Name}' does not match '{sectionName}'");
        return null;
    }
}
