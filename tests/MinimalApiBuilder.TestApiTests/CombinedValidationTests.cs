using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation;
using MinimalApiBuilder.TestApiTests.Infrastructure;
using NUnit.Framework;

namespace MinimalApiBuilder.TestApiTests;

internal sealed class CombinedValidationTests
{
    [TestCaseSource(nameof(Invalid))]
    public async Task Invalid_Request(
        AsyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PutAsJsonAsync($"/validation/combination?value={parameters.Value}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(Valid))]
    public async Task Valid_Request(
        AsyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PutAsJsonAsync($"/validation/combination?value={parameters.Value}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    public static readonly object[] Invalid =
    [
        new object[] { new AsyncValidationRequest("a"), new SyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest("b"), new SyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest("c"), new SyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest("true"), new SyncValidationParameters(3) }
    ];

    public static readonly object[] Valid =
    [
        new object[] { new AsyncValidationRequest("true"), new SyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest("true"), new SyncValidationParameters(4) }
    ];
}
