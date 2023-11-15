using Microsoft.CodeAnalysis;
using MinimalApiBuilder.Generator.Common;

namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerateHandler : IToGenerate
{
    public EndpointToGenerateHandler(
        ISymbol handler,
        EndpointToGenerateHandlerParameter? endpointParameter,
        EndpointToGenerateHandlerParameter[] parameters)
    {
        Symbol = handler;
        Name = handler.Name;
        EndpointParameter = endpointParameter;
        Parameters = parameters;
    }

    public ISymbol Symbol { get; }

    public string Name { get; }

    public EndpointToGenerateHandlerParameter? EndpointParameter { get; }

    public EndpointToGenerateHandlerParameter[] Parameters { get; }
}
