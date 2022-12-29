using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTest;

public static class TestHelper
{
    public static async Task AssertErrorResultAsync(HttpResponseMessage response)
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        ErrorResult? errorResult = await response.Content.ReadFromJsonAsync<ErrorResult>();

        Assert.That(errorResult, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(errorResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(errorResult.Message, Has.Length.GreaterThan(0));
            Assert.That(errorResult.Errors, Has.Length.GreaterThan(0));
        });
    }
}
