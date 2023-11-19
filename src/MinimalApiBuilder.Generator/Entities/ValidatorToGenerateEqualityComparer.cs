namespace MinimalApiBuilder.Generator.Entities;

internal sealed class ValidatorToGenerateEqualityComparer : IEqualityComparer<ValidatorToGenerate>
{
    public static readonly ValidatorToGenerateEqualityComparer Instance = new();

    public bool Equals(ValidatorToGenerate? x, ValidatorToGenerate? y)
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

        return x.ValidatedType == y.ValidatedType &&
               x.IsAsync == y.IsAsync &&
               x.ServiceLifetime == y.ServiceLifetime;
    }

    public int GetHashCode(ValidatorToGenerate obj)
    {
        throw new NotImplementedException();
    }
}
