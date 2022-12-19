namespace Web.Features.Admin.Login;

public class AdminLogin : MinimalApiBuilder.Endpoint<Request>
{
    private readonly ILogger<AdminLogin> _logger;

    public AdminLogin(ILogger<AdminLogin> logger)
    {
        _logger = logger;
    }

    protected override Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin Login");

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
}
