using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Async;
using Parameters = Fixture.TestApi.Features.Validation.Sync.Parameters;

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
    public async Task Combined_Validation_With_Invalid_Request(Request request, Parameters parameters)
    {
        HttpResponseMessage response =
            await _client.PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [Theory]
    [MemberData(nameof(Valid))]
    public async Task Combined_Validation_With_Valid_Request(Request request, Parameters parameters)
    {
        HttpResponseMessage response =
            await _client.PutAsJsonAsync($"/validation/combination?bar={parameters.Bar}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public static readonly IEnumerable<object[]> Invalid = new[]
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

    public static readonly IEnumerable<object[]> Valid = new[]
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
