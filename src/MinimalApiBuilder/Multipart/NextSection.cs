using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using MinimalApiBuilder.Generator;

namespace MinimalApiBuilder.Multipart;

/// <summary>
/// A wrapper around <see cref="MultipartSection" />.
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
    /// Converts the current section to a <see cref="FormMultipartSection" />.
    /// </summary>
    /// <returns>
    /// The converted <see cref="FormMultipartSection" /> or <c>null</c> if the current section is not form data.
    /// </returns>
    /// <remarks>
    /// Validation errors are added to <see cref="MinimalApiBuilderEndpoint.ValidationErrors" />.
    /// </remarks>
    public FormMultipartSection? AsFormSection()
    {
        if (_section.IsFormData(out ContentDispositionHeaderValue? contentDisposition))
        {
            return new FormMultipartSection(_section, contentDisposition);
        }

        _endpoint.AddMultipartError(
            $"The multipart section with the name '{contentDisposition?.Name}' is not form data");

        return null;
    }

    /// <summary>
    /// Converts the current section to a <see cref="FormMultipartSection" /> and validates its name.
    /// </summary>
    /// <param name="sectionName">
    /// The required form-section name.
    /// </param>
    /// <returns>
    /// The converted <see cref="FormMultipartSection" /> or <c>null</c> if the current section is not form data.
    /// </returns>
    /// <remarks>
    /// Validation errors are added to <see cref="MinimalApiBuilderEndpoint.ValidationErrors" />.
    /// </remarks>
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

        _endpoint.AddMultipartError(
            $"The multipart section with the name '{section.Name}' does not match the expected name '{sectionName}'");

        return null;
    }

    /// <summary>
    /// Checks if the current section is a <see cref="FormMultipartSection" />.
    /// </summary>
    /// <returns></returns>
    public bool IsFormSection()
    {
        return _section.IsFormData(out _contentDispositionHeader);
    }

    /// <summary>
    /// Converts the current section to a <see cref="FormMultipartSection" /> without validation.
    /// </summary>
    /// <returns></returns>
    public FormMultipartSection AsFormSectionDangerous()
    {
        return new FormMultipartSection(_section, _contentDispositionHeader);
    }

    /// <summary>
    /// Converts the current section to a <see cref="FileMultipartSection" />.
    /// </summary>
    /// <returns>
    /// The converted <see cref="FileMultipartSection" /> or <c>null</c> if the current section is not a file.
    /// </returns>
    /// <remarks>
    /// Validation errors are added to <see cref="MinimalApiBuilderEndpoint.ValidationErrors" />.
    /// </remarks>
    public FileMultipartSection? AsFileSection()
    {
        if (_section.IsFile(out ContentDispositionHeaderValue? contentDisposition))
        {
            return new FileMultipartSection(_section, contentDisposition);
        }

        _endpoint.AddMultipartError(
            $"The multipart section with the name '{contentDisposition?.Name}' is not a file");

        return null;
    }

    /// <summary>
    /// Converts the current section to a <see cref="FileMultipartSection" /> and validates its name.
    /// </summary>
    /// <param name="sectionName">The required file-section name.</param>
    /// <returns>
    /// The converted <see cref="FileMultipartSection" /> or <c>null</c> if the current section is not a file.
    /// </returns>
    /// <remarks>
    /// Validation errors are added to <see cref="MinimalApiBuilderEndpoint.ValidationErrors" />.
    /// </remarks>
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

        _endpoint.AddMultipartError(
            $"The multipart section with the name '{section.Name}' does not match the expected name '{sectionName}'");

        return null;
    }

    /// <summary>
    /// Checks if the current section is a <see cref="FileMultipartSection" />.
    /// </summary>
    /// <returns></returns>
    public bool IsFileSection()
    {
        return _section.IsFile(out _contentDispositionHeader);
    }

    /// <summary>
    /// Converts the current section to a <see cref="FileMultipartSection" /> without validation.
    /// </summary>
    /// <returns></returns>
    public FileMultipartSection AsFileSectionDangerous()
    {
        return new FileMultipartSection(_section, _contentDispositionHeader);
    }
}
