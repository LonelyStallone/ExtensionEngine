namespace ExtensionEngine.Abstractions.Gateway.Models;

public class EnvelopMessage
{
    public required MessageInfo Info { get; init; }

    public required MessagePayload Payload { get; init; }
}
