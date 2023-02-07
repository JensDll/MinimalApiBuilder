using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Asynchronous;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTest.Validation;

public class AsyncValidationTests
{
    [TestCaseSource(nameof(_invalidSingle))]
    public async Task Single_Parameter_Validation_With_Invalid_Request(Request request)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PostAsJsonAsync("/validation/async/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(_validSingle))]
    public async Task Single_Parameter_Validation_With_Valid_Request(Request request)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PostAsJsonAsync("/validation/async/single", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCaseSource(nameof(_invalidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Invalid_Request(Request request,
        Parameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PatchAsJsonAsync($"/validation/async/multiple?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(_validMultiple))]
    public async Task Multiple_Parameters_Validation_With_Valid_Request(Request request,
        Parameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PatchAsJsonAsync($"/validation/async/multiple?bar={parameters.Bar}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private static object[] _invalidSingle =
    {
        new object[] { new Request { Foo = "invalid" } },
        new object[] { new Request { Foo = "false" } },
        new object[] { new Request { Foo = "no" } }
    };

    private static object[] _validSingle =
    {
        new object[] { new Request { Foo = "valid" } },
        new object[] { new Request { Foo = "also valid" } }
    };

    private static object[] _invalidMultiple =
    {
        new object[] { new Request { Foo = "invalid" }, new Parameters(2) },
        new object[] { new Request { Foo = "false" }, new Parameters(3) },
        new object[] { new Request { Foo = "no" }, new Parameters(2) },
        new object[] { new Request { Foo = "valid" }, new Parameters(3) }
    };

    private static object[] _validMultiple =
    {
        new object[] { new Request { Foo = "valid" }, new Parameters(2) },
        new object[] { new Request { Foo = "also valid" }, new Parameters(4) }
    };
}
