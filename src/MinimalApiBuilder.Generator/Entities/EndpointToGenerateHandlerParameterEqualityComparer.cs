namespace MinimalApiBuilder.Generator.Entities;

internal sealed class EndpointToGenerateHandlerParameterEqualityComparer :
    IEqualityComparer<EndpointToGenerateHandlerParameter>
{
    public static readonly EndpointToGenerateHandlerParameterEqualityComparer Instance = new();

    public bool Equals(EndpointToGenerateHandlerParameter? left, EndpointToGenerateHandlerParameter? right)
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

        return left.Position == right.Position &&
               left.IsNullable == right.IsNullable &&
               left.IsValueType == right.IsValueType &&
               left.HasCustomBinding == right.HasCustomBinding &&
               left.NeedsCustomBindingNullCheck == right.NeedsCustomBindingNullCheck;
    }

    public int GetHashCode(EndpointToGenerateHandlerParameter obj)
    {
        throw new NotImplementedException();
    }
}
