// ReSharper disable StaticMemberInGenericType

namespace MinimalApiBuilder;

public abstract partial class Endpoint<TEndpoint> : EndpointBase, IEndpoint
    where TEndpoint : EndpointBase, IEndpoint
{
    public static Dictionary<Type, int> ArgumentPositions { get; } = new();
}
