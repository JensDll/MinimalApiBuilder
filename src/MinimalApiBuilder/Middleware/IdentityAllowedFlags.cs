namespace MinimalApiBuilder.Middleware;

[Flags]
internal enum IdentityAllowedFlags : byte
{
    None = 0,
    Allowed = 1,
    NotAllowed = 2
}
