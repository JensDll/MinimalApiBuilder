using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace MinimalApiBuilder;

public class NextSection
{
    private readonly MultipartSection _section;

    public NextSection(MultipartSection section)
    {
        _section = section;
    }

    public FormMultipartSection? AsFormSection()
    {
        return _section.IsFormData(out ContentDispositionHeaderValue? contentDisposition)
            ? new FormMultipartSection(_section, contentDisposition)
            : null;
    }

    public FormMultipartSection? AsFormSection(string sectionName)
    {
        FormMultipartSection? section = AsFormSection();

        if (section is null)
        {
            return null;
        }

        return section.Name == sectionName ? section : null;
    }

    public FileMultipartSection? AsFileSection()
    {
        return _section.IsFile(out ContentDispositionHeaderValue? contentDisposition)
            ? new FileMultipartSection(_section, contentDisposition)
            : null;
    }

    public FileMultipartSection? AsFileSection(string sectionName)
    {
        FileMultipartSection? section = AsFileSection();

        if (section is null)
        {
            return null;
        }

        return section.Name == sectionName ? section : null;
    }
}
