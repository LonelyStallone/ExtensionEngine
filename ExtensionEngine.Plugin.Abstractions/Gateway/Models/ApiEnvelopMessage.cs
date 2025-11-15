using ExtensionEngine.Abstractions.Plugin.Models;

namespace ExtensionEngine.Abstractions.Gateway.Models;

public class ApiEnvelopMessage
{
    public required string HostId { get; init; }

    public required PluginInfo PluginInfo { get; init; }

    public required Guid MessageId { get; init; }

    public required Guid TraceId { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required string PayloadType { get; init; }

    public required string Payload { get; init; }
}
