using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Async;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests.Tests;

public class AsyncValidationTests
{
    [TestCaseSource(nameof(InvalidSingle))]
    public async Task Single_Parameter_Validation_With_Invalid_Request(AsyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client.PostAsJsonAsync("/validation/async/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(ValidSingle))]
    public async Task Single_Parameter_Validation_With_Valid_Request(AsyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client.PostAsJsonAsync("/validation/async/single", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCaseSource(nameof(InvalidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Invalid_Request(
        AsyncValidationRequest request,
        AsyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PatchAsJsonAsync($"/validation/async/multiple?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(ValidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Valid_Request(
        AsyncValidationRequest request,
        AsyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PatchAsJsonAsync($"/validation/async/multiple?bar={parameters.Bar}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    public static readonly object[] InvalidSingle =
    {
        new object[] { new AsyncValidationRequest { Foo = "invalid" } },
        new object[] { new AsyncValidationRequest { Foo = "false" } },
        new object[] { new AsyncValidationRequest { Foo = "no" } }
    };

    public static readonly object[] ValidSingle =
    {
        new object[] { new AsyncValidationRequest { Foo = "valid" } },
        new object[] { new AsyncValidationRequest { Foo = "also valid" } }
    };

    public static readonly object[] InvalidMultiple =
    {
        new object[] { new AsyncValidationRequest { Foo = "invalid" }, new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "false" }, new AsyncValidationParameters(3) },
        new object[] { new AsyncValidationRequest { Foo = "no" }, new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "valid" }, new AsyncValidationParameters(3) }
    };

    public static readonly object[] ValidMultiple =
    {
        new object[] { new AsyncValidationRequest { Foo = "valid" }, new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "also valid" }, new AsyncValidationParameters(4) }
    };
}
