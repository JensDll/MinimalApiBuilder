namespace MinimalApiBuilder;

[Flags]
public enum HttpVerbs
{
    Get = 1,
    Post = 2,
    Put = 4,
    Patch = 8,
    Delete = 16
}
