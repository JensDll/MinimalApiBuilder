namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerateHandlerEqualityComparer : IEqualityComparer<EndpointToGenerateHandler>
{
    public static readonly EndpointToGenerateHandlerEqualityComparer Instance = new();

    public bool Equals(EndpointToGenerateHandler? x, EndpointToGenerateHandler? y)
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

        if (x.Name != y.Name)
        {
            return false;
        }

        var parameterComparer = EndpointToGenerateHandlerParameterEqualityComparer.Instance;

        if (!parameterComparer.Equals(x.EndpointParameter, y.EndpointParameter))
        {
            return false;
        }

        if (x.Parameters.Length != y.Parameters.Length)
        {
            return false;
        }

        for (int i = 0; i < x.Parameters.Length; ++i)
        {
            if (!parameterComparer.Equals(x.Parameters[i], y.Parameters[i]))
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(EndpointToGenerateHandler obj)
    {
        throw new NotImplementedException();
    }
}
