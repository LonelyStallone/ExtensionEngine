namespace ExtensionEngine.Abstractions.Gateway.Models;

public class MessagePayload
{
    public required Type Type { get; init; }

    public required string Data { get; init; }
}
