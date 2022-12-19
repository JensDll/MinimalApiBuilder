using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public abstract partial class EndpointBase
{
    protected internal HttpContext HttpContext { get; internal set; } = null!;

    protected internal virtual void Configure() { }
}
