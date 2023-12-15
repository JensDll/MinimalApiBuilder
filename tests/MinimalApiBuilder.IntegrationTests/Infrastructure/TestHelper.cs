using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests.Infrastructure;

internal static class TestHelper
{
    public static async Task AssertErrorResultAsync(HttpResponseMessage response)
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        ProblemDetails? problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(problemDetails, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(problemDetails!.Status, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(problemDetails.Extensions, Has.Count.GreaterThan(0));
            Assert.That(problemDetails.Extensions, Does.ContainKey("errors"));
        });
    }
}
