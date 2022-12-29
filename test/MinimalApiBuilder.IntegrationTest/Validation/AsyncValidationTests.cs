using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;
using Sample.WebApi.Features.Validation.Async;

namespace MinimalApiBuilder.IntegrationTest.Validation;

public class AsyncValidationTests
{
    [TestCaseSource(nameof(_invalidSingle))]
    public async Task Single_Parameter_Validation_With_Invalid_Request(AsyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PostAsJsonAsync("/validation/async/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(_validSingle))]
    public async Task Single_Parameter_Validation_With_Valid_Request(AsyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PostAsJsonAsync("/validation/async/single", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCaseSource(nameof(_invalidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Invalid_Request(AsyncValidationRequest request,
        AsyncValidationParameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PatchAsJsonAsync($"/validation/async/multiple?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(_validMultiple))]
    public async Task Multiple_Parameters_Validation_With_Valid_Request(AsyncValidationRequest request,
        AsyncValidationParameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PatchAsJsonAsync($"/validation/async/multiple?bar={parameters.Bar}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private static object[] _invalidSingle =
    {
        new object[] { new AsyncValidationRequest { Foo = "invalid" } },
        new object[] { new AsyncValidationRequest { Foo = "false" } },
        new object[] { new AsyncValidationRequest { Foo = "no" } }
    };

    private static object[] _validSingle =
    {
        new object[] { new AsyncValidationRequest { Foo = "valid" } },
        new object[] { new AsyncValidationRequest { Foo = "also valid" } }
    };

    private static object[] _invalidMultiple =
    {
        new object[] { new AsyncValidationRequest { Foo = "invalid" }, new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "false" }, new AsyncValidationParameters(3) },
        new object[] { new AsyncValidationRequest { Foo = "no" }, new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "valid" }, new AsyncValidationParameters(3) }
    };

    private static object[] _validMultiple =
    {
        new object[] { new AsyncValidationRequest { Foo = "valid" }, new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "also valid" }, new AsyncValidationParameters(4) }
    };
}
