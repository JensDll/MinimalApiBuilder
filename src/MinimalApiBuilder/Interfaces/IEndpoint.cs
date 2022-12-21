using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public interface IEndpoint
{
    internal static abstract Dictionary<Type, int> ArgumentPositions { get; }

    internal IResult ErrorResult(string message);
}
