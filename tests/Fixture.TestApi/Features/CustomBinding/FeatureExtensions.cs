using Fixture.TestApi.Features.CustomBinding.BindAsync;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApiBuilder;

namespace Fixture.TestApi.Features.CustomBinding;

public static class FeatureExtensions
{
    public static void MapCustomBindingFeatures(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/bind").WithTags("Custom Binding");
        group.MapPost<BindAsyncEndpoint>("/bindasync");
    }
}
