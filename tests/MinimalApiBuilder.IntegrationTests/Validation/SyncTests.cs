using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Sync;
using Xunit;

namespace MinimalApiBuilder.IntegrationTests;

[Collection(HttpClientCollectionFixture.Name)]
public class SyncValidationTests
{
    private readonly HttpClient _client;

    public SyncValidationTests(HttpClientFixture fixture)
    {
        _client = fixture.Client;
    }

    [Theory]
    [MemberData(nameof(InvalidSingle))]
    public async Task Single_Parameter_Validation_With_Invalid_Request(SyncValidationRequest request)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/validation/sync/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [Theory]
    [MemberData(nameof(ValidSingle))]
    public async Task Single_Parameter_Validation_With_Valid_Request(SyncValidationRequest request)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/validation/sync/single", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(InvalidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Invalid_Request(
        SyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await _client.PatchAsJsonAsync($"/validation/sync/multiple?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [Theory]
    [MemberData(nameof(ValidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Valid_Request(
        SyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response =
            await _client.PatchAsJsonAsync($"/validation/sync/multiple?bar={parameters.Bar}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public static readonly IEnumerable<object[]> InvalidSingle = new[]
    {
        new object[] { new SyncValidationRequest() { Foo = "invalid" } },
        new object[] { new SyncValidationRequest { Foo = "false" } },
        new object[] { new SyncValidationRequest { Foo = "no" } }
    };

    public static readonly IEnumerable<object[]> ValidSingle = new[]
    {
        new object[] { new SyncValidationRequest { Foo = "valid" } },
        new object[] { new SyncValidationRequest { Foo = "also valid" } }
    };

    public static readonly IEnumerable<object[]> InvalidMultiple = new[]
    {
        new object[] { new SyncValidationRequest { Foo = "invalid" }, new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest { Foo = "valid" }, new SyncValidationParameters(3) },
        new object[] { new SyncValidationRequest { Foo = "false" }, new SyncValidationParameters(3) },
        new object[] { new SyncValidationRequest { Foo = "no" }, new SyncValidationParameters(2) }
    };

    public static readonly IEnumerable<object[]> ValidMultiple = new[]
    {
        new object[] { new SyncValidationRequest { Foo = "valid" }, new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest { Foo = "also valid" }, new SyncValidationParameters(4) }
    };
}
