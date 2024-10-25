namespace ChatService.IntegrationTests.Responses;

public class ErrorMessage
{
    public string Type { get; init; } = null!;

    public string Title { get; init; } = null!;

    public int Status { get; init; }

    public string TraceId { get; init; } = null!;

    public string Message { get; init; } = null!;

    public string Code { get; init; } = null!;
}
