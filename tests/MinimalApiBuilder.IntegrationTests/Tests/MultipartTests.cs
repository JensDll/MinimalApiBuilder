using System.IO.Compression;
using System.Text;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests.Tests;

internal sealed class MultipartTests
{
    [Test]
    public async Task Zip_Stream_Request()
    {
        using MultipartFormDataContent formData = new();

        string textData = new('a', 56);
        using StringContent textContent = new(textData);
        formData.Add(textContent, "some_text");

        byte[] fileData = "The quick brown fox jumps over the lazy dog"u8.ToArray();
        using StreamContent fileContent = new(new MemoryStream(fileData));
        formData.Add(fileContent, "some_file", "some_file.txt");

        Uri uri = new("/multipart/zipstream", UriKind.Relative);
        HttpResponseMessage response = await TestSetup.Client.PostAsync(uri, formData);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        await using Stream responseStream = await response.Content.ReadAsStreamAsync();
        using ZipArchive archive = new(responseStream, ZipArchiveMode.Read);

        Assert.That(archive.Entries, Has.Count.EqualTo(2));
        Assert.That(archive.Entries[0].Name, Is.EqualTo("some_text"));
        Assert.That(archive.Entries[1].Name, Is.EqualTo("some_file.txt"));

        using StreamReader textReader = new(archive.Entries[0].Open(), Encoding.UTF8);
        string text = await textReader.ReadToEndAsync();
        Assert.That(text, Is.EqualTo(textData));

        using StreamReader fileReader = new(archive.Entries[1].Open(), Encoding.UTF8);
        string file = await fileReader.ReadToEndAsync();
        Assert.That(file, Is.EqualTo("The quick brown fox jumps over the lazy dog"));
    }
}
