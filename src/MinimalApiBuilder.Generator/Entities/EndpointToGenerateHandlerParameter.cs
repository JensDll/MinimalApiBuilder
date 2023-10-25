namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerateHandlerParameter
{
    private readonly string _identifier;

    public EndpointToGenerateHandlerParameter(string identifier, int position)
    {
        _identifier = identifier;
        Position = position;
    }

    public int Position { get; }

    public override string ToString() => _identifier;
}
