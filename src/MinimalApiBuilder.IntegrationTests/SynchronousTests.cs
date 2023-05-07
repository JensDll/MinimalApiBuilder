﻿using System.Net;
using System.Net.Http.Json;
using Fixture.TestApi.Features.Validation.Sync;

namespace MinimalApiBuilder.IntegrationTests;

[Collection(HttpClientCollection.Name)]
public class SyncValidationTests
{
    private readonly HttpClient _client;

    public SyncValidationTests(HttpClientFixture fixture)
    {
        _client = fixture.Client;
    }

    [Theory]
    [MemberData(nameof(InvalidSingle))]
    public async Task Single_Parameter_Validation_With_Invalid_Request(Request request)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/validation/sync/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [Theory]
    [MemberData(nameof(ValidSingle))]
    public async Task Single_Parameter_Validation_With_Valid_Request(Request request)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("/validation/sync/single", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(InvalidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Invalid_Request(Request request,
        Parameters parameters)
    {
        HttpResponseMessage response =
            await _client.PatchAsJsonAsync($"/validation/sync/multiple?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [Theory]
    [MemberData(nameof(ValidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Valid_Request(Request request,
        Parameters parameters)
    {
        HttpResponseMessage response =
            await _client.PatchAsJsonAsync($"/validation/sync/multiple?bar={parameters.Bar}", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public static IEnumerable<object[]> InvalidSingle = new[]
    {
        new object[] { new Request { Foo = "invalid" } },
        new object[] { new Request { Foo = "false" } },
        new object[] { new Request { Foo = "no" } }
    };

    public static IEnumerable<object[]> ValidSingle = new[]
    {
        new object[] { new Request { Foo = "valid" } },
        new object[] { new Request { Foo = "also valid" } }
    };

    public static IEnumerable<object[]> InvalidMultiple = new[]
    {
        new object[] { new Request { Foo = "invalid" }, new Parameters(2) },
        new object[] { new Request { Foo = "valid" }, new Parameters(3) },
        new object[] { new Request { Foo = "false" }, new Parameters(3) },
        new object[] { new Request { Foo = "no" }, new Parameters(2) }
    };

    public static IEnumerable<object[]> ValidMultiple = new[]
    {
        new object[] { new Request { Foo = "valid" }, new Parameters(2) },
        new object[] { new Request { Foo = "also valid" }, new Parameters(4) }
    };
}
