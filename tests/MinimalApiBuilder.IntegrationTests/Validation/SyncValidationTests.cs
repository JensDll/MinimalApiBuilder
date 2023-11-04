using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Sync;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests;

public class SyncValidationTests
{
    [TestCaseSource(nameof(InvalidSingle))]
    public async Task Single_Parameter_Validation_With_Invalid_Request(SyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client.PostAsJsonAsync("/validation/sync/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(ValidSingle))]
    public async Task Single_Parameter_Validation_With_Valid_Request(SyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client.PostAsJsonAsync("/validation/sync/single", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCaseSource(nameof(InvalidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Invalid_Request(
        SyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PatchAsJsonAsync($"/validation/sync/multiple?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(ValidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Valid_Request(
        SyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PatchAsJsonAsync($"/validation/sync/multiple?bar={parameters.Bar}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    public static readonly object[] InvalidSingle =
    {
        new SyncValidationRequest { Foo = "invalid" },
        new SyncValidationRequest { Foo = "false" },
        new SyncValidationRequest { Foo = "no" }
    };

    public static readonly object[] ValidSingle =
    {
        new SyncValidationRequest() { Foo = "valid" },
        new SyncValidationRequest() { Foo = "also valid" }
    };

    public static readonly object[] InvalidMultiple =
    {
        new object[] { new SyncValidationRequest { Foo = "invalid" }, new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest { Foo = "valid" }, new SyncValidationParameters(3) },
        new object[] { new SyncValidationRequest { Foo = "false" }, new SyncValidationParameters(3) },
        new object[] { new SyncValidationRequest { Foo = "no" }, new SyncValidationParameters(2) }
    };

    public static readonly object[] ValidMultiple =
    {
        new object[] { new SyncValidationRequest { Foo = "valid" }, new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest { Foo = "also valid" }, new SyncValidationParameters(4) }
    };
}
