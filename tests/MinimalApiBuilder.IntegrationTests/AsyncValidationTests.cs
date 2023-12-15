using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation;
using MinimalApiBuilder.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests;

internal sealed class AsyncValidationTests
{
    private static readonly string[] s_multipartErrors =
        ["Missing content-type boundary", "Content-Type must be multipart/form-data"];

    [TestCaseSource(nameof(InvalidSingle))]
    public async Task Invalid_Single_Parameter(AsyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client.PostAsJsonAsync("/api/validation/async/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(ValidSingle))]
    public async Task Valid_Single_Parameter(AsyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client.PostAsJsonAsync("/api/validation/async/single", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result!["multipart"], Is.EquivalentTo(s_multipartErrors));
    }

    [TestCaseSource(nameof(InvalidMultiple))]
    public async Task Invalid_Multiple_Parameters(
        AsyncValidationRequest request,
        AsyncValidationParameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client.PatchAsJsonAsync(
            $"/api/validation/async/multiple?value={parameters.Value}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(ValidMultiple))]
    public async Task Valid_Multiple_Parameters(
        AsyncValidationRequest request,
        AsyncValidationParameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client.PatchAsJsonAsync(
            $"/api/validation/async/multiple?value={parameters.Value}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    public static readonly object[] InvalidSingle =
    [
        new AsyncValidationRequest("a"),
        new AsyncValidationRequest("b"),
        new AsyncValidationRequest("c")
    ];

    public static readonly object[] InvalidMultiple =
    [
        new object[] { new AsyncValidationRequest("a"), new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest("b"), new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest("c"), new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest("true"), new AsyncValidationParameters(3) }
    ];

    public static readonly object[] ValidSingle =
    [
        new AsyncValidationRequest("true")
    ];

    public static readonly object[] ValidMultiple =
    [
        new object[] { new AsyncValidationRequest("true"), new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest("true"), new AsyncValidationParameters(4) }
    ];
}
