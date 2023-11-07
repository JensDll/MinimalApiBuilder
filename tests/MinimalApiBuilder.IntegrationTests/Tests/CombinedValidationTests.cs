using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Async;
using Fixture.TestApi.Features.Validation.Sync;
using NUnit.Framework;

namespace MinimalApiBuilder.IntegrationTests.Tests;

internal sealed class CombinedValidationTests
{
    [TestCaseSource(nameof(Invalid))]
    public async Task Combined_Validation_With_Invalid_Request(
        AsyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(Valid))]
    public async Task Combined_Validation_With_Valid_Request(
        AsyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await TestSetup.Client.PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    public static readonly object[] Invalid =
    {
        new object[]
        {
            new AsyncValidationRequest { Foo = "invalid" },
            new SyncValidationParameters(2)
        },
        new object[]
        {
            new AsyncValidationRequest { Foo = "false" },
            new SyncValidationParameters(4)
        },
        new object[]
        {
            new AsyncValidationRequest { Foo = "no" },
            new SyncValidationParameters(3)
        },
        new object[]
        {
            new AsyncValidationRequest { Foo = "valid" },
            new SyncValidationParameters(3)
        }
    };

    public static readonly object[] Valid =
    {
        new object[]
        {
            new AsyncValidationRequest { Foo = "valid" },
            new SyncValidationParameters(2)
        },
        new object[]
        {
            new AsyncValidationRequest { Foo = "also valid" },
            new SyncValidationParameters(4)
        }
    };
}
