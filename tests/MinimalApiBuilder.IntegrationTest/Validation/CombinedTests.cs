using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Asynchronous;
using NUnit.Framework;
using Parameters = Fixture.TestApi.Features.Validation.Synchronous.Parameters;

namespace MinimalApiBuilder.IntegrationTest.Validation;

public class CombinedValidationTests
{
    [TestCaseSource(nameof(_invalid))]
    public async Task Combined_Validation_With_Invalid_Request(Request request, Parameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(_valid))]
    public async Task Combined_Validation_With_Valid_Request(Request request, Parameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private static object[] _invalid =
    {
        new object[]
        {
            new Request { Foo = "invalid" }, new Parameters(2)
        },
        new object[]
        {
            new Request { Foo = "false" }, new Parameters(4)
        },
        new object[]
        {
            new Request { Foo = "no" }, new Parameters(3)
        },
        new object[]
        {
            new Request { Foo = "valid" }, new Parameters(3)
        }
    };

    private static object[] _valid =
    {
        new object[]
        {
            new Request { Foo = "valid" }, new Parameters(2)
        },
        new object[]
        {
            new Request { Foo = "also valid" }, new Parameters(4)
        }
    };
}
