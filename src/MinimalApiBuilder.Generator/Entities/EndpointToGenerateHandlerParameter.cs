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

internal class EndpointToGenerateHandlerParameterEqualityComparer :
    IEqualityComparer<EndpointToGenerateHandlerParameter>
{
    public static readonly EndpointToGenerateHandlerParameterEqualityComparer Instance = new();

    public bool Equals(EndpointToGenerateHandlerParameter? x, EndpointToGenerateHandlerParameter? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        return x.Position == y.Position;
    }

    public int GetHashCode(EndpointToGenerateHandlerParameter obj)
    {
        throw new NotImplementedException();
    }
}
