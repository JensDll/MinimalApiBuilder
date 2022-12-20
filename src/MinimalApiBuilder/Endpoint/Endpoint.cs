using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

[EndpointType(EndpointType.Empty)]
public abstract class Endpoint : EndpointBase
{
    protected static class ArgumentIndex
    {
        public const int Endpoint = 0;
        public const int CancellationToken = 1;
    }

    internal Task<IResult> ExecuteAsync(CancellationToken cancellationToken)
    {
        return HandleAsync(cancellationToken);
    }

    protected internal abstract Task<IResult> HandleAsync(CancellationToken cancellationToken);
}

[EndpointType(EndpointType.Request)]
public abstract class Endpoint<TRequest> : EndpointBase
    where TRequest : notnull
{
    protected static class ArgumentIndex
    {
        public const int Request = 0;
        public const int Endpoint = 1;
        public const int CancellationToken = 2;
    }

    internal async Task<IResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
    {
        bool isValid = await ValidateAsync(request, cancellationToken);

        if (!isValid)
        {
            return ErrorResult("Validation failed");
        }

        return await HandleAsync(request, cancellationToken);
    }

    protected abstract Task<IResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

[EndpointType(EndpointType.Parameters)]
public abstract class EndpointWithParameters<TParameters> : EndpointBase
    where TParameters : notnull
{
    protected static class ArgumentIndex
    {
        public const int Parameters = 0;
        public const int Endpoint = 1;
        public const int CancellationToken = 2;
    }

    internal static Task<IResult> RequestHandler(TParameters parameters,
        EndpointWithParameters<TParameters> endpoint,
        CancellationToken cancellationToken) =>
        endpoint.ExecuteAsync(parameters, cancellationToken);

    internal Task<IResult> ExecuteAsync(TParameters parameters, CancellationToken cancellationToken)
    {
        return HandleAsync(parameters, cancellationToken);
    }

    protected abstract Task<IResult> HandleAsync(TParameters parameters, CancellationToken cancellationToken);
}

[EndpointType(EndpointType.RequestWithParameters)]
public abstract class EndpointWithParameters<TRequest, TParameters> : EndpointBase
    where TRequest : notnull
    where TParameters : notnull
{
    protected static class ArgumentIndex
    {
        public const int Request = 0;
        public const int Parameters = 1;
        public const int Endpoint = 2;
        public const int CancellationToken = 3;
    }

    internal async Task<IResult> ExecuteAsync(TRequest request, TParameters parameters,
        CancellationToken cancellationToken)
    {
        bool isValid = await ValidateAsync(request, cancellationToken);

        if (!isValid)
        {
            return ErrorResult("Validation failed");
        }

        return await HandleAsync(request, parameters, cancellationToken);
    }

    protected abstract Task<IResult> HandleAsync(TRequest request, TParameters parameters,
        CancellationToken cancellationToken);
}
