namespace MinimalApiBuilder.IntegrationTest;

public class ErrorResult
{
    public required int StatusCode { get; init; }

    public required string Message { get; init; }

    public required string[] Errors { get; init; }
}
