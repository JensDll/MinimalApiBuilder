namespace MinimalApiBuilder;

public interface IEndpointHandler
{
    public static abstract Delegate Handler { get; }
}
