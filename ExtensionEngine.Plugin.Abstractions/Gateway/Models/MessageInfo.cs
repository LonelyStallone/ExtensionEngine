namespace ExtensionEngine.Abstractions.Gateway.Models;

public class MessageInfo
{
    public required Guid SpanId { get; init; }

    public required Guid TraceId { get; init; }

    public required DateTime CreatedAt { get; init; }
}
