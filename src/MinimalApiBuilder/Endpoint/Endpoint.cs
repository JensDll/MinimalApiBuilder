using Microsoft.AspNetCore.Http;

namespace MinimalApiBuilder;

[EndpointType(EndpointType.Empty)]
public abstract class Endpoint : EndpointBase
{
    internal Task<IResult> ExecuteAsync(HttpContext context, CancellationToken cancellationToken)
    {
        HttpContext = context;
        return HandleAsync(cancellationToken);
    }

    protected internal abstract Task<IResult> HandleAsync(CancellationToken cancellationToken);
}

[EndpointType(EndpointType.Request)]
public abstract class Endpoint<TRequest> : EndpointBase
    where TRequest : notnull
{
    internal async Task<IResult> ExecuteAsync(TRequest request, HttpContext context,
        CancellationToken cancellationToken)
    {
        HttpContext = context;

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
    internal Task<IResult> ExecuteAsync(TParameters parameters, HttpContext context,
        CancellationToken cancellationToken)
    {
        HttpContext = context;
        return HandleAsync(parameters, cancellationToken);
    }

    protected abstract Task<IResult> HandleAsync(TParameters parameters, CancellationToken cancellationToken);
}

[EndpointType(EndpointType.RequestWithParameters)]
public abstract class EndpointWithParameters<TRequest, TParameters> : EndpointBase
    where TRequest : notnull
    where TParameters : notnull
{
    internal async Task<IResult> ExecuteAsync(TRequest request, TParameters parameters, HttpContext context,
        CancellationToken cancellationToken)
    {
        HttpContext = context;

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
