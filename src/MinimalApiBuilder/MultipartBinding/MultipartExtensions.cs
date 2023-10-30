using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace MinimalApiBuilder;

internal static class MultipartExtensions
{
    public static string GetBoundary(this HttpContext context)
    {
        if (context.Request.ContentType is null)
        {
            throw new MultipartBindingException("Missing content-type header");
        }

        MediaTypeHeaderValue contentType = MediaTypeHeaderValue.Parse(context.Request.ContentType);
        string? boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

        if (string.IsNullOrWhiteSpace(boundary))
        {
            throw new MultipartBindingException("Missing content-type boundary");
        }

        IOptions<FormOptions> formOptions = context.RequestServices.GetRequiredService<IOptions<FormOptions>>();
        int lengthLimit = formOptions.Value.MultipartBoundaryLengthLimit;

        if (boundary.Length > lengthLimit)
        {
            throw new MultipartBindingException(
                $"Multipart boundary length limit '{lengthLimit}' exceeded");
        }

        return boundary;
    }

    public static bool IsMultipart(this HttpContext context)
    {
        return !string.IsNullOrEmpty(context.Request.ContentType) &&
               context.Request.ContentType.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsFormData(this MultipartSection section,
        [NotNullWhen(true)] out ContentDispositionHeaderValue? contentDisposition)
    {
        bool hasContentDispositionHeader =
            ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

        return hasContentDispositionHeader &&
               contentDisposition is not null &&
               contentDisposition.DispositionType.Equals("form-data", StringComparison.OrdinalIgnoreCase) &&
               string.IsNullOrEmpty(contentDisposition.FileName.Value) &&
               string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
    }

    public static bool IsFile(this MultipartSection section,
        [NotNullWhen(true)] out ContentDispositionHeaderValue? contentDisposition)
    {
        bool hasContentDispositionHeader =
            ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

        return hasContentDispositionHeader &&
               contentDisposition is not null &&
               contentDisposition.DispositionType.Equals("form-data", StringComparison.OrdinalIgnoreCase) &&
               (!string.IsNullOrEmpty(contentDisposition.FileName.Value) ||
                !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
    }
}
