using MinimalApiBuilder;
using ILogger = Serilog.ILogger;

namespace Web.Features.Admin.Login;

public class AdminLogin : Endpoint<AdminLogin>
{
    private readonly ILogger _logger;

    public AdminLogin(ILogger logger)
    {
        _logger = logger;
    }

    protected override void Configure(RouteHandlerBuilder builder)
    {
        Validate<Request, RequestValidator>(builder);
        builder.AddEndpointFilter(LogFilter);
    }

    public static IResult Handle(Request request, AdminLogin endpoint)
    {
        endpoint._logger.Information("[AdminLogin] Executing {Handler}", nameof(Handle));

        if (request is not { UserName: "admin", Password: "admin" })
        {
            endpoint.AddValidationError("Invalid request");
            endpoint._logger.Information("[AdminLogin] Has validation errors {Value}",
                endpoint.HasValidationErrors);
            return endpoint.ErrorResult("Authentication failed");
        }

        DateTime expiryDate = DateTime.UtcNow.AddDays(1);

        return Results.Ok(new Response
        {
            JwtToken = "Some Token",
            ExpiryDate = expiryDate,
            Permissions = new[] { "Admin" }
        });
    }

    private static async ValueTask<object?> LogFilter(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        Request request = context.GetArgument<Request>(0);
        AdminLogin adminLogin = context.GetArgument<AdminLogin>(1);

        adminLogin._logger.Information("[AdminLogin] Filter before (UserName = {UserName})",
            request.UserName);

        var result = await next(context);

        adminLogin._logger.Information("[AdminLogin] Filter after (UserName = {UserName})",
            request.UserName);

        return result;
    }
}
