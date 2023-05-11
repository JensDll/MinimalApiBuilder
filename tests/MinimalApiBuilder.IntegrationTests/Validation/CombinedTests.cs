using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Async;
using Fixture.TestApi.Features.Validation.Sync;
using Xunit;

namespace MinimalApiBuilder.IntegrationTests;

[Collection(HttpClientCollectionFixture.Name)]
public class CombinedValidationTests
{
    private readonly HttpClient _client;

    public CombinedValidationTests(HttpClientFixture fixture)
    {
        _client = fixture.Client;
    }

    [Theory]
    [MemberData(nameof(Invalid))]
    public async Task Combined_Validation_With_Invalid_Request(
        AsyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await _client.PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [Theory]
    [MemberData(nameof(Valid))]
    public async Task Combined_Validation_With_Valid_Request(
        AsyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await _client.PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public static readonly IEnumerable<object[]> Invalid = new[]
    {
        new object[]
        {
            new AsyncValidationRequest() { Foo = "invalid" },
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

    public static readonly IEnumerable<object[]> Valid = new[]
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
