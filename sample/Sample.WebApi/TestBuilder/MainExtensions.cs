// using MinimalApiBuilder;
//
// namespace Sample.WebApi.TestBuilder;
//
// public static class MainExtensions
// {
//     public static RouteHandlerBuilder MPost<TEndpoint>(this IEndpointRouteBuilder app, string pattern)
//         where TEndpoint : IEndpointHandler
//     {
//         return app
//             .MapPost(pattern, TEndpoint.Handler);
//
//     }
//
//     public static RouteHandlerBuilder MapPost_Sample_WebApi_Features_Validation_Sync_ValidationSync(this IEndpointRouteBuilder app, string pattern)
//     {
//         return app
//             .MapPost(pattern, Sample.WebApi.Features.Validation.Sync.ValidationSync.Handle);
//
//     }
//
//     private static Delegate GetHandler<TEndpoint>()
//         where TEndpoint : IEndpointHandler => TEndpoint.Handler;
// }
//
// public interface IEndpointHandler
// {
//     public static abstract Delegate Handler { get; }
// }



