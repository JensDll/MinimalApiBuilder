using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;
using Sample.WebApi.Features.Validation.Async;
using Sample.WebApi.Features.Validation.Sync;

namespace MinimalApiBuilder.IntegrationTest.Validation;

public class CombinedValidationTests
{
    [TestCaseSource(nameof(_invalid))]
    public async Task Combined_Validation_With_Invalid_Request(AsyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(_valid))]
    public async Task Combined_Validation_With_Valid_Request(AsyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private static object[] _invalid =
    {
        new object[] { new AsyncValidationRequest { Foo = "invalid" }, new SyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "false" }, new SyncValidationParameters(4) },
        new object[] { new AsyncValidationRequest { Foo = "no" }, new SyncValidationParameters(3) },
        new object[] { new AsyncValidationRequest { Foo = "valid" }, new SyncValidationParameters(3) }
    };

    private static object[] _valid =
    {
        new object[] { new AsyncValidationRequest { Foo = "valid" }, new SyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "also valid" }, new SyncValidationParameters(4) }
    };
}
