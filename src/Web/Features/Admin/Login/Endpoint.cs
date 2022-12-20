using ILogger = Serilog.ILogger;

namespace Web.Features.Admin.Login;

public class AdminLogin : MinimalApiBuilder.Endpoint<Request>
{
    private readonly ILogger _logger;

    public AdminLogin(ILogger logger)
    {
        _logger = logger;
    }

    protected override void Configure(RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(TestFilter);
    }

    protected override Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        _logger.Information("[AdminLogin] HandleAsync");

        if (request is not { UserName: "admin", Password: "admin" })
        {
            AddValidationError("Invalid request");
            return Task.FromResult(ErrorResult("Authentication failed"));
        }

        DateTime expiryDate = DateTime.UtcNow.AddDays(1);

        return Task.FromResult(Results.Ok(new Response
        {
            JwtToken = "Some Token",
            ExpiryDate = expiryDate,
            Permissions = new[] { "Admin" }
        }));
    }

    private static async ValueTask<object?> TestFilter(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        Request request = context.GetArgument<Request>(ArgumentIndex.Request);
        AdminLogin adminLogin = context.GetArgument<AdminLogin>(ArgumentIndex.Endpoint);
        CancellationToken cancellationToken = context.GetArgument<CancellationToken>(ArgumentIndex.CancellationToken);

        adminLogin._logger.Information("[AdminLogin] Filter before");

        object? result = await next(context);

        adminLogin._logger.Information("[AdminLogin] Filter after");

        return result;
    }
}
