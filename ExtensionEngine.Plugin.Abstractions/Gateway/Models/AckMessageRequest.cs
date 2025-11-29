namespace ExtensionEngine.Abstractions.Gateway.Models;

public class AckMessageRequest : IMessageRequest
{
    public required Guid MessageId { get; init; }
}
