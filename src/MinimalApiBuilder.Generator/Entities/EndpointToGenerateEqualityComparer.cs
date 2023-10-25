namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerateEqualityComparer : IEqualityComparer<EndpointToGenerate>
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

        var handlerComparer = EndpointToGenerateHandlerEqualityComparer.Instance;

        return x.ClassName == y.ClassName &&
               x.NamespaceName == y.NamespaceName &&
               handlerComparer.Equals(x.Handler, y.Handler) &&
               x.NeedsConfigure == y.NeedsConfigure;
    }

    public int GetHashCode(EndpointToGenerate obj)
    {
        throw new NotImplementedException();
    }
}
