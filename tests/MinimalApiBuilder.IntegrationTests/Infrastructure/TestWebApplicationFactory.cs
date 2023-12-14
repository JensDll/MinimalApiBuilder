using Microsoft.AspNetCore.Mvc.Testing;

namespace MinimalApiBuilder.IntegrationTests.Infrastructure;

internal sealed class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class;
