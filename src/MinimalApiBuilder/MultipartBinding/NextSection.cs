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

    private ContentDispositionHeaderValue? _contentDispositionHeader;

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

        _endpoint.AddValidationError(
            $"The multipart section with the name '{contentDisposition?.Name}' is not form data");
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

        _endpoint.AddValidationError(
            $"The multipart section with the name '{section.Name}' does not match the expected name '{sectionName}'");
        return null;
    }

    /// <summary>
    /// Checks if the section is a form section.
    /// </summary>
    /// <returns></returns>
    public bool IsFormSection()
    {
        return _section.IsFormData(out _contentDispositionHeader);
    }

    /// <summary>
    /// Converts itself to a form section without validation.
    /// </summary>
    /// <returns></returns>
    public FormMultipartSection AsFormSectionDangerous()
    {
        return new FormMultipartSection(_section, _contentDispositionHeader);
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

        _endpoint.AddValidationError($"The multipart section with the name '{contentDisposition?.Name}' is not a file");
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

        _endpoint.AddValidationError(
            $"The multipart section with the name '{section.Name}' does not match the expected name '{sectionName}'");
        return null;
    }

    /// <summary>
    /// Checks if the section is a file section.
    /// </summary>
    /// <returns></returns>
    public bool IsFileSection()
    {
        return _section.IsFile(out _contentDispositionHeader);
    }

    /// <summary>
    /// Converts itself to a file section without validation.
    /// </summary>
    /// <returns></returns>
    public FileMultipartSection AsFileSectionDangerous()
    {
        return new FileMultipartSection(_section, _contentDispositionHeader);
    }
}
