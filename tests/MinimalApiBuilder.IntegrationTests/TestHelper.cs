using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests;

public static class TestHelper
{
    public static async Task AssertErrorResultAsync(HttpResponseMessage response)
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        ErrorDto? errorResult = await response.Content.ReadFromJsonAsync<ErrorDto>();

        Assert.NotNull(errorResult);

        Assert.Multiple(() =>
        {
            Assert.That(errorResult!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.True(errorResult.Message.Length > 0);
            Assert.True(errorResult.Errors.Any());
        });
    }
}
