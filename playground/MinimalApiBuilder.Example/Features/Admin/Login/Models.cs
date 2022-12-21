using FluentValidation;

namespace Web.Features.Admin.Login;

public class Request
{
    public required string UserName { get; init; }
    public required string Password { get; init; }
}

public class Response
{
    public required string JwtToken { get; init; }
    public required DateTime ExpiryDate { get; init; }
    public required IEnumerable<string> Permissions { get; init; }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(request => request.UserName)
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters");

        RuleFor(request => request.UserName)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username is too short");

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(3).WithMessage("Password is too short");
    }
}
