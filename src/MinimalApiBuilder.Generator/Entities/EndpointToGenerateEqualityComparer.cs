namespace MinimalApiBuilder.Generator.Entities;

internal sealed class EndpointToGenerateEqualityComparer : IEqualityComparer<EndpointToGenerate>
{
    public static readonly EndpointToGenerateEqualityComparer Instance = new();

    public bool Equals(EndpointToGenerate? x, EndpointToGenerate? y)
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

        return x.ClassName == y.ClassName &&
               x.NamespaceName == y.NamespaceName &&
               x.NeedsConfigure == y.NeedsConfigure &&
               x.Accessibility == y.Accessibility &&
               EndpointToGenerateHandlerEqualityComparer.Instance.Equals(x.Handler, y.Handler);
    }

    public int GetHashCode(EndpointToGenerate obj)
    {
        throw new NotImplementedException();
    }
}
