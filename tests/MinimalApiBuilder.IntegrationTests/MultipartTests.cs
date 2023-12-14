using System.IO.Compression;
using System.Net.Http.Json;
using System.Text;
using Fixture.TestApi.Features.Multipart;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests;

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

        Uri uri = new("/api/multipart/zipstream", UriKind.Relative);
        HttpResponseMessage response = await TestSetup.Client.PostAsync(uri, formData);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        await using Stream responseStream = await response.Content.ReadAsStreamAsync();
        using ZipArchive archive = new(responseStream, ZipArchiveMode.Read);

        Assert.That(archive.Entries, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(archive.Entries[0].Name, Is.EqualTo("some_text"));
            Assert.That(archive.Entries[1].Name, Is.EqualTo("some_file.txt"));
        });

        using StreamReader textReader = new(archive.Entries[0].Open(), Encoding.UTF8);
        string text = await textReader.ReadToEndAsync();
        Assert.That(text, Is.EqualTo(textData));

        using StreamReader fileReader = new(archive.Entries[1].Open(), Encoding.UTF8);
        string file = await fileReader.ReadToEndAsync();
        Assert.That(file, Is.EqualTo("The quick brown fox jumps over the lazy dog"));
    }

    [Test]
    public async Task Buffered_Files_Request()
    {
        using MultipartFormDataContent formData = new();

        byte[] data1 = new byte[100];
        TestContext.CurrentContext.Random.NextBytes(data1);
        using StreamContent fileContent1 = new(new MemoryStream(data1));
        formData.Add(fileContent1, "some_file_1", "some_file_1.bin");

        byte[] data2 = new byte[100];
        TestContext.CurrentContext.Random.NextBytes(data2);
        using StreamContent fileContent2 = new(new MemoryStream(data2));
        formData.Add(fileContent2, "some_file_2", "some_file_2.bin");

        byte[] data3 = new byte[100];
        TestContext.CurrentContext.Random.NextBytes(data3);
        using StreamContent fileContent3 = new(new MemoryStream(data3));
        formData.Add(fileContent3, "some_file_3", "some_file_3.bin");

        Uri uri = new("/api/multipart/bufferedfiles", UriKind.Relative);
        HttpResponseMessage response = await TestSetup.Client.PostAsync(uri, formData);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        BufferedFilesResponse[]? result = await response.Content.ReadFromJsonAsync<BufferedFilesResponse[]>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(result![0].Name, Is.EqualTo("some_file_1.bin"));
            Assert.That(result[0].Data, Is.EqualTo(data1));
            Assert.That(result[1].Name, Is.EqualTo("some_file_2.bin"));
            Assert.That(result[1].Data, Is.EqualTo(data2));
            Assert.That(result[2].Name, Is.EqualTo("some_file_3.bin"));
            Assert.That(result[2].Data, Is.EqualTo(data3));
        });
    }
}
