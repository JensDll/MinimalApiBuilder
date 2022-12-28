using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;
using Sample.WebApi.Features.Validation.Synchronous;

namespace MinimalApiBuilder.IntegrationTest.Validation;

public class SynchronousValidationTests
{
    private static object[] _invalidRequestsSingle =
    {
        new object[] { new SyncValidationRequest { Foo = "" } },
        new object[] { new SyncValidationRequest { Foo = "F" } }
    };

    private static object[] _invalidRequestsMultiple =
    {
        new object[] { new SyncValidationRequest { Foo = "" }, new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest { Foo = "Foo" }, new SyncValidationParameters(3) },
        new object[] { new SyncValidationRequest { Foo = "F" }, new SyncValidationParameters(3) }
    };

    [TestCaseSource(nameof(_invalidRequestsSingle))]
    public async Task SinglePostWithInvalidRequest(SyncValidationRequest request)
    {
        HttpResponseMessage response = await TestContext.Client
            .PostAsJsonAsync("/validation/sync/single", request);

        await AssertBadRequestAsync(response);
    }

    [TestCaseSource(nameof(_invalidRequestsMultiple))]
    public async Task MultiplePatchWithInvalidRequest(SyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response = await TestContext.Client
            .PatchAsJsonAsync($"/validation/sync/multiple?bar={parameters.Bar}", request);

        await AssertBadRequestAsync(response);
    }

    private static async Task AssertBadRequestAsync(HttpResponseMessage response)
    {
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        ErrorResult? errorResult = await response.Content.ReadFromJsonAsync<ErrorResult>();

        Assert.That(errorResult, Is.Not.Null);
        Assert.That(errorResult!.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        Assert.That(errorResult.Message, Has.Length.GreaterThan(0));
        Assert.That(errorResult.Errors, Has.Length.GreaterThan(0));
    }
}
