namespace MinimalApiBuilder.IntegrationTests;

public class HttpClientFixture : IDisposable
{
    public HttpClientFixture()
    {
        Factory = new TestWebApplicationFactory<Program>();
        Client = Factory.CreateClient();
    }

    internal TestWebApplicationFactory<Program> Factory { get; }

    internal HttpClient Client { get; }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Factory.Dispose();
        Client.Dispose();
    }
}
