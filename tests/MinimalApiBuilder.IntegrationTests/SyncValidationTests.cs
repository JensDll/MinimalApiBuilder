using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation;
using MinimalApiBuilder.TestApiTests.Infrastructure;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests;

internal sealed class SyncValidationTests
{
    [TestCaseSource(nameof(InvalidSingle))]
    public async Task Invalid_Single_Parameter(SyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client.PostAsJsonAsync("/validation/sync/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(ValidSingle))]
    public async Task Valid_Single_Parameter(SyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client.PostAsJsonAsync("/validation/sync/single", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCaseSource(nameof(InvalidMultiple))]
    public async Task Invalid_Multiple_Parameters(
        SyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PatchAsJsonAsync($"/validation/sync/multiple?value={parameters.Value}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(ValidMultiple))]
    public async Task Valid_Multiple_Parameters(
        SyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PatchAsJsonAsync($"/validation/sync/multiple?value={parameters.Value}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    public static readonly object[] InvalidSingle =
    [
        new SyncValidationRequest("a"),
        new SyncValidationRequest("b"),
        new SyncValidationRequest("c")
    ];

    public static readonly object[] InvalidMultiple =
    [
        new object[] { new SyncValidationRequest("a"), new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest("b"), new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest("c"), new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest("true"), new SyncValidationParameters(3) }
    ];

    public static readonly object[] ValidSingle =
    [
        new SyncValidationRequest("true")
    ];

    public static readonly object[] ValidMultiple =
    [
        new object[] { new SyncValidationRequest("true"), new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest("true"), new SyncValidationParameters(4) }
    ];
}
