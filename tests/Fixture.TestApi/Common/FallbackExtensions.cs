using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApiBuilder.Middleware;

namespace Fixture.TestApi.Common;

internal static class FallbackExtensions
{
    private static readonly string[] s_supportedHttpMethods = [HttpMethods.Get, HttpMethods.Head];

    public static IEndpointConventionBuilder MapFallbackToIndexHtml(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapFallback(CreateRequestDelegate(endpoints))
            .WithMetadata(new HttpMethodMetadata(s_supportedHttpMethods));

        static RequestDelegate CreateRequestDelegate(IEndpointRouteBuilder endpoints)
        {
            IApplicationBuilder app = endpoints.CreateApplicationBuilder();

            app.Use(static next => context =>
            {
                context.Request.Path += "/index.html";
                context.SetEndpoint(null);
                return next(context);
            });

            app.UseCompressedStaticFiles();

            return app.Build();
        }
    }
}
