using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Async;
using Xunit;

namespace MinimalApiBuilder.IntegrationTests;

[Collection(HttpClientCollectionFixture.Name)]
public class AsyncValidationTests
{
    private readonly HttpClient _client;

    public AsyncValidationTests(HttpClientFixture fixture)
    {
        _client = fixture.Client;
    }

    [Theory]
    [MemberData(nameof(InvalidSingle))]
    public async Task Single_Parameter_Validation_With_Invalid_Request(AsyncValidationRequest request)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/validation/async/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [Theory]
    [MemberData(nameof(ValidSingle))]
    public async Task Single_Parameter_Validation_With_Valid_Request(AsyncValidationRequest request)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/validation/async/single", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(InvalidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Invalid_Request(
        AsyncValidationRequest request,
        AsyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await _client.PatchAsJsonAsync($"/validation/async/multiple?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [Theory]
    [MemberData(nameof(ValidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Valid_Request(
        AsyncValidationRequest request,
        AsyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await _client.PatchAsJsonAsync($"/validation/async/multiple?bar={parameters.Bar}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public static readonly IEnumerable<object[]> InvalidSingle = new[]
    {
        new object[] { new AsyncValidationRequest { Foo = "invalid" } },
        new object[] { new AsyncValidationRequest { Foo = "false" } },
        new object[] { new AsyncValidationRequest { Foo = "no" } }
    };

    public static readonly IEnumerable<object[]> ValidSingle = new[]
    {
        new object[] { new AsyncValidationRequest { Foo = "valid" } },
        new object[] { new AsyncValidationRequest { Foo = "also valid" } }
    };

    public static readonly IEnumerable<object[]> InvalidMultiple = new[]
    {
        new object[] { new AsyncValidationRequest { Foo = "invalid" }, new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "false" }, new AsyncValidationParameters(3) },
        new object[] { new AsyncValidationRequest { Foo = "no" }, new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "valid" }, new AsyncValidationParameters(3) }
    };

    public static readonly IEnumerable<object[]> ValidMultiple = new[]
    {
        new object[] { new AsyncValidationRequest { Foo = "valid" }, new AsyncValidationParameters(2) },
        new object[] { new AsyncValidationRequest { Foo = "also valid" }, new AsyncValidationParameters(4) }
    };
}
