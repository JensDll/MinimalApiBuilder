namespace MinimalApiBuilder.Generator.Common;

public static class Extensions
{
    public static string ServiceLifetimeToString(this int value)
    {
        return value switch
        {
            0 => "Singleton",
            1 => "Scoped",
            2 => "Transient",
            _ => "Singleton"
        };
    }
}
