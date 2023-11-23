using Microsoft.CodeAnalysis;

namespace MinimalApiBuilder.Generator.Entities;

internal sealed class EndpointToGenerateHandler
{
    public EndpointToGenerateHandler(
        ISymbol handler,
        EndpointToGenerateHandlerParameter? endpointParameter,
        EndpointToGenerateHandlerParameter[] parameters)
    {
        Name = handler.Name;
        EndpointParameter = endpointParameter;
        Parameters = parameters;
    }

    public string Name { get; }

    public EndpointToGenerateHandlerParameter? EndpointParameter { get; }

    public EndpointToGenerateHandlerParameter[] Parameters { get; }
}
