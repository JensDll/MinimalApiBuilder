namespace MinimalApiBuilder.Generator.Entities;

internal sealed class ConfigureToGenerateEqualityComparer : IEqualityComparer<ConfigureToGenerate>
{
    public static readonly ConfigureToGenerateEqualityComparer Instance = new();

    public bool Equals(ConfigureToGenerate? x, ConfigureToGenerate? y)
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

        return x.Arity == y.Arity &&
               x.FilePath == y.FilePath &&
               x.LineNumber == y.LineNumber &&
               EndpointsEquals(x, y);
    }

    public int GetHashCode(ConfigureToGenerate obj)
    {
        throw new NotImplementedException();
    }

    private static bool EndpointsEquals(ConfigureToGenerate x, ConfigureToGenerate y)
    {
        if (x.Endpoints.Count != y.Endpoints.Count)
        {
            return false;
        }

        for (int i = 0; i < x.Endpoints.Count; ++i)
        {
            if (x.Endpoints[i] != y.Endpoints[i])
            {
                return false;
            }
        }

        return true;
    }
}
