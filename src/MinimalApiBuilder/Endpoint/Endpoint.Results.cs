﻿using System.Diagnostics.Contracts;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public abstract partial class Endpoint<TEndpoint>
    where TEndpoint : EndpointBase, IEndpoint
{
    [Pure]
    public IResult ErrorResult(string message)
    {
        return Results.BadRequest(new
        {
            StatusCode = HttpStatusCode.BadRequest,
            Message = message,
            Errors = ValidationErrors
        });
    }

    [Pure]
    public IResult ErrorResult(string message, HttpStatusCode statusCode)
    {
        return Results.BadRequest(new
        {
            StatusCode = statusCode,
            Message = message,
            Errors = ValidationErrors
        });
    }

    [Pure]
    public IResult BodyWriterStreamResult(Func<Stream, Task> streamWriterCallback,
        string? contentType = null,
        string? fileDownloadName = null)
    {
        return new BodyWriterStreamResult(streamWriterCallback, contentType, fileDownloadName);
    }
}
