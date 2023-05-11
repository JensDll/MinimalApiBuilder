using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MinimalApiBuilder.IntegrationTests;

public static class TestHelper
{
    public static async Task AssertErrorResultAsync(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        ErrorDto? errorResult = await response.Content.ReadFromJsonAsync<ErrorDto>();

        Assert.NotNull(errorResult);

        Assert.Multiple(() =>
        {
            Assert.Equal(HttpStatusCode.BadRequest, errorResult.StatusCode);
            Assert.True(errorResult.Message.Length > 0);
            Assert.True(errorResult.Errors.Any());
        });
    }
}
