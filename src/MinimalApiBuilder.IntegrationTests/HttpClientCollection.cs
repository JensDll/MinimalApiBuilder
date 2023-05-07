namespace MinimalApiBuilder.IntegrationTests;

[CollectionDefinition(Name)]
public class HttpClientCollection : ICollectionFixture<HttpClientFixture>
{
    public const string Name = "HttpClient Collcetion";
}
