namespace MinimalApiBuilder.Generator.Entities;

internal sealed class ConfigureToGenerateEqualityComparer : IEqualityComparer<ConfigureToGenerate>
{
    public static readonly ConfigureToGenerateEqualityComparer Instance = new();

    public bool Equals(ConfigureToGenerate? left, ConfigureToGenerate? right)
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

        return left.Arity == right.Arity &&
               left.FilePath == right.FilePath &&
               left.LineNumber == right.LineNumber &&
               EndpointsEquals(left, right);
    }

    public int GetHashCode(ConfigureToGenerate obj)
    {
        throw new NotImplementedException();
    }

    private static bool EndpointsEquals(ConfigureToGenerate left, ConfigureToGenerate right)
    {
        if (left.Endpoints.Count != right.Endpoints.Count)
        {
            return false;
        }

        for (int i = 0; i < left.Endpoints.Count; ++i)
        {
            if (left.Endpoints[i] != right.Endpoints[i])
            {
                return false;
            }
        }

        return true;
    }
}
