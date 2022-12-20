using System.Net;
using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

public interface IEndpoint
{
    internal static abstract Dictionary<Type, int> ArgumentPositions { get; }

    IResult ErrorResult(string message);

    IResult ErrorResult(string message, HttpStatusCode statusCode);
}
