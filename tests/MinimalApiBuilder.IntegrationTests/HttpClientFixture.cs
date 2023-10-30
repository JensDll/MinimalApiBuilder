namespace MinimalApiBuilder.IntegrationTests;

public sealed class HttpClientFixture : IDisposable
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public HttpClientFixture()
    {
        _factory = new TestWebApplicationFactory<Program>();
        Client = _factory.CreateClient();
    }

    internal HttpClient Client { get; }

    public void Dispose()
    {
        _factory.Dispose();
        Client.Dispose();
    }
}
