using Microsoft.AspNetCore.Mvc.Testing;

namespace MinimalApiBuilder.TestApiTests.Infrastructure;

internal sealed class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class;
