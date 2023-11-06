using Microsoft.AspNetCore.Builder;

namespace MinimalApiBuilder;

public interface IEndpoint
{
#pragma warning disable CA1707
#pragma warning disable IDE1006
    static abstract Delegate _auto_generated_Handler { get; }

    static abstract void _auto_generated_Configure(RouteHandlerBuilder builder);
#pragma warning restore IDE1006
#pragma warning restore CA1707

    static abstract void Configure(RouteHandlerBuilder builder);
}
