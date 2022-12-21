// ReSharper disable StaticMemberInGenericType

namespace MinimalApiBuilder;

public abstract partial class Endpoint<TEndpoint> : EndpointBase, IEndpoint
    where TEndpoint : EndpointBase, IEndpoint
{
    static Dictionary<Type, int> IEndpoint.ArgumentPositions { get; } = new();
}
