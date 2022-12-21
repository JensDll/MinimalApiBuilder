using Microsoft.AspNetCore.Builder;

namespace MinimalApiBuilder;

public abstract class EndpointBase
{
    internal List<string> ValidationErrors { get; } = new();
    protected internal EndpointConfiguration Configuration { get; internal set; } = null!;

    protected internal virtual void Configure(RouteHandlerBuilder builder) { }
}
