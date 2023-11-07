using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests;

internal static class TestHelper
{
    public static async Task AssertErrorResultAsync(HttpResponseMessage response)
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        ErrorDto? errorResult = await response.Content.ReadFromJsonAsync<ErrorDto>();
        Assert.That(errorResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(errorResult!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(errorResult.Message, Is.Not.Null);
            Assert.That(errorResult.Errors, Is.Not.Empty);
        });
    }
}
