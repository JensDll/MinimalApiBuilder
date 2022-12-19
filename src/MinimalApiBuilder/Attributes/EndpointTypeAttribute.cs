namespace MinimalApiBuilder;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class EndpointTypeAttribute : Attribute
{
    public EndpointType Type { get; }

    public EndpointTypeAttribute(EndpointType type)
    {
        Type = type;
    }
}
