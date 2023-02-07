﻿using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;
using Sample.WebApi.Features.Validation.Sync;

namespace MinimalApiBuilder.IntegrationTest.Validation;

public class SyncValidationTests
{
    [TestCaseSource(nameof(_invalidSingle))]
    public async Task Single_Parameter_Validation_With_Invalid_Request(SyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PostAsJsonAsync("/validation/sync/single", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(_validSingle))]
    public async Task Single_Parameter_Validation_With_Valid_Request(SyncValidationRequest request)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PostAsJsonAsync("/validation/sync/single", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [TestCaseSource(nameof(_invalidMultiple))]
    public async Task Multiple_Parameters_Validation_With_Invalid_Request(SyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PatchAsJsonAsync($"/validation/sync/multiple?bar={parameters.Bar}", request);

        await TestHelper.AssertErrorResultAsync(response);
    }

    [TestCaseSource(nameof(_validMultiple))]
    public async Task Multiple_Parameters_Validation_With_Valid_Request(SyncValidationRequest request,
        SyncValidationParameters parameters)
    {
        HttpResponseMessage response = await TestSetup.Client
            .PatchAsJsonAsync($"/validation/sync/multiple?bar={parameters.Bar}", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    private static object[] _invalidSingle =
    {
        new object[] { new SyncValidationRequest { Foo = "invalid" } },
        new object[] { new SyncValidationRequest { Foo = "false" } },
        new object[] { new SyncValidationRequest { Foo = "no" } }
    };

    private static object[] _validSingle =
    {
        new object[] { new SyncValidationRequest { Foo = "valid" } },
        new object[] { new SyncValidationRequest { Foo = "also valid" } }
    };

    private static object[] _invalidMultiple =
    {
        new object[] { new SyncValidationRequest { Foo = "invalid" }, new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest { Foo = "valid" }, new SyncValidationParameters(3) },
        new object[] { new SyncValidationRequest { Foo = "false" }, new SyncValidationParameters(3) },
        new object[] { new SyncValidationRequest { Foo = "no" }, new SyncValidationParameters(2) }
    };

    private static object[] _validMultiple =
    {
        new object[] { new SyncValidationRequest { Foo = "valid" }, new SyncValidationParameters(2) },
        new object[] { new SyncValidationRequest { Foo = "also valid" }, new SyncValidationParameters(4) }
    };
}
