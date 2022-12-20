using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public abstract partial class EndpointBase
{
    protected HttpContext HttpContext { get; private set; } = null!;

    protected internal virtual void Configure(RouteHandlerBuilder builder) { }

    internal void Assign(EndpointConfiguration configuration, HttpContext httpContext)
    {
        HttpContext = httpContext;
    }
}
