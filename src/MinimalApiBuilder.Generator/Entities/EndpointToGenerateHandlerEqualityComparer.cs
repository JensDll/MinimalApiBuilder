namespace MinimalApiBuilder.Generator.Entities;

internal sealed class EndpointToGenerateHandlerEqualityComparer : IEqualityComparer<EndpointToGenerateHandler>
{
    public static readonly EndpointToGenerateHandlerEqualityComparer Instance = new();

    public bool Equals(EndpointToGenerateHandler? left, EndpointToGenerateHandler? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null)
        {
            return false;
        }

        if (right is null)
        {
            return false;
        }

        if (left.Name != right.Name)
        {
            return false;
        }

        if (!EndpointToGenerateHandlerParameterEqualityComparer.Instance.Equals(
            left.EndpointParameter, right.EndpointParameter))
        {
            return false;
        }

        if (left.Parameters.Length != right.Parameters.Length)
        {
            return false;
        }

        for (int i = 0; i < left.Parameters.Length; ++i)
        {
            if (!EndpointToGenerateHandlerParameterEqualityComparer.Instance.Equals(
                left.Parameters[i], right.Parameters[i]))
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
