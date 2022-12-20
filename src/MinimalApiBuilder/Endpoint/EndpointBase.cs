using Microsoft.AspNetCore.Builder;

namespace MinimalApiBuilder;

public abstract class EndpointBase
{
    internal EndpointConfiguration Configuration { get; set; } = null!;
    protected internal List<string> ValidationErrors { get; } = new();

    protected internal virtual void Configure(RouteHandlerBuilder builder) { }
}
