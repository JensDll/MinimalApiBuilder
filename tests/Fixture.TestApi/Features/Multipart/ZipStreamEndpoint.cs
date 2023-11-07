﻿using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.Multipart;

internal partial class ZipStreamEndpoint : MinimalApiBuilderEndpoint
{
    private static async Task Handle(
        ZipStreamEndpoint endpoint,
        ZipStreamRequest request,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        using ZipArchive archive = new(context.Response.BodyWriter.AsStream(), ZipArchiveMode.Create);

        while (await request.MultipartReader.ReadNextSectionAsync(cancellationToken) is { } nextSection)
        {
            if (nextSection.IsFormSection())
            {
                await ProcessAsync(archive, nextSection.AsFormSectionDangerous());
                continue;
            }

            if (nextSection.IsFileSection())
            {
                await ProcessAsync(archive, nextSection.AsFileSectionDangerous());
            }
        }
    }

    public static void Configure(RouteHandlerBuilder builder)
    { }

    private static async Task ProcessAsync(ZipArchive archive, FormMultipartSection section)
    {
        ZipArchiveEntry entry = archive.CreateEntry(section.Name, CompressionLevel.Fastest);
        await using Stream entryStream = entry.Open();
        await section.Section.Body.CopyToAsync(entryStream);
    }

    private static async Task ProcessAsync(ZipArchive archive, FileMultipartSection section)
    {
        ZipArchiveEntry entry = archive.CreateEntry(section.FileName, CompressionLevel.Fastest);
        await using Stream entryStream = entry.Open();
        await section.FileStream!.CopyToAsync(entryStream);
    }
}

internal class ZipStreamRequest
{
    public required MinimalApiBuilder.MultipartReader MultipartReader { get; init; }

    public static ValueTask<ZipStreamRequest?> BindAsync(HttpContext context)
    {
        ZipStreamEndpoint endpoint = context.RequestServices.GetRequiredService<ZipStreamEndpoint>();
        MinimalApiBuilder.MultipartReader multipartReader = new(context, endpoint);

        if (endpoint.HasValidationError)
        {
            return ValueTask.FromResult<ZipStreamRequest?>(null);
        }

        return ValueTask.FromResult<ZipStreamRequest?>(new ZipStreamRequest
        {
            MultipartReader = multipartReader
        });
    }
}
