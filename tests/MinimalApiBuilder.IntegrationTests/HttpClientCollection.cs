using Xunit;

namespace MinimalApiBuilder.IntegrationTests;

[CollectionDefinition(Name)]
public class HttpClientCollectionFixture : ICollectionFixture<HttpClientFixture>
{
    public const string Name = "HttpClient Collcetion";
}
