namespace MinimalApiBuilder.Generator.Entities;

internal sealed class EndpointToGenerateEqualityComparer : IEqualityComparer<EndpointToGenerate>
{
    public static readonly EndpointToGenerateEqualityComparer Instance = new();

    public bool Equals(EndpointToGenerate? left, EndpointToGenerate? right)
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

        return left.ClassName == right.ClassName &&
               left.NamespaceName == right.NamespaceName &&
               left.NeedsConfigure == right.NeedsConfigure &&
               left.Accessibility == right.Accessibility &&
               EndpointToGenerateHandlerEqualityComparer.Instance.Equals(left.Handler, right.Handler);
    }

    public int GetHashCode(EndpointToGenerate obj)
    {
        throw new NotImplementedException();
    }
}
