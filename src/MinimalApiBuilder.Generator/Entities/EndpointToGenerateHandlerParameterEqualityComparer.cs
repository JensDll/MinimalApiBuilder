namespace MinimalApiBuilder.Generator.Entities;

internal sealed class EndpointToGenerateHandlerParameterEqualityComparer :
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

        return x.Position == y.Position &&
               x.IsNullable == y.IsNullable &&
               x.IsValueType == y.IsValueType &&
               x.HasCustomBinding == y.HasCustomBinding;
    }

    public int GetHashCode(EndpointToGenerateHandlerParameter obj)
    {
        throw new NotImplementedException();
    }
}
