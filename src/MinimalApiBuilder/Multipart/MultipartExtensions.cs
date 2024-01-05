using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using MinimalApiBuilder.Generator;

namespace MinimalApiBuilder.Multipart;

internal static class MultipartExtensions
{
    internal static string GetBoundary(this HttpContext context, MinimalApiBuilderEndpoint endpoint,
        FormOptions formOptions)
    {
        if (context.Request.ContentType is null)
        {
            endpoint.AddMultipartError("Missing content-type header");
            return string.Empty;
        }

        MediaTypeHeaderValue contentType = MediaTypeHeaderValue.Parse(context.Request.ContentType);
        string? boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

        if (string.IsNullOrWhiteSpace(boundary))
        {
            endpoint.AddMultipartError("Missing content-type boundary");
            return string.Empty;
        }

        int lengthLimit = formOptions.MultipartBoundaryLengthLimit;

        if (boundary.Length <= lengthLimit)
        {
            return boundary;
        }

        endpoint.AddMultipartError($"Multipart boundary length limit '{lengthLimit}' exceeded");
        return string.Empty;
    }

    internal static bool IsMultipart(this HttpContext context)
    {
        return !string.IsNullOrEmpty(context.Request.ContentType) &&
               context.Request.ContentType.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase);
    }

    internal static bool IsFormData(this MultipartSection section,
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

    internal static bool IsFile(this MultipartSection section,
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

    internal static void AddMultipartError(this MinimalApiBuilderEndpoint endpoint, string message)
    {
        endpoint.AddValidationError("multipart", message);
    }
}
