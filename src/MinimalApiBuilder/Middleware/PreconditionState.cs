namespace MinimalApiBuilder.Middleware;

internal enum PreconditionState : byte
{
    ShouldProcess,
    NotModified,
    PreconditionFailed
}
