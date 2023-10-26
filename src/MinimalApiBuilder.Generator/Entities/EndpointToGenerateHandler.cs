namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerateHandler
{
    public EndpointToGenerateHandler(string name, EndpointToGenerateHandlerParameter endpointParameter,
        EndpointToGenerateHandlerParameter[] parameters)
    {
        Name = name;
        EndpointParameter = endpointParameter;
        Parameters = parameters;
    }

    public string Name { get; }

    public EndpointToGenerateHandlerParameter EndpointParameter { get; }

    public EndpointToGenerateHandlerParameter[] Parameters { get; }
}
