using ExtensionEngine.Abstractions.Gateway.Models;
using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Plugin.Abstractions.Gateway;

public interface IBackendGateway
{
    Task ProduceApiEnvelopMessageAsync<TEntry>(ApiEnvelopMessage message);

    Task ProduceApiEnvelopMessagesAsync<TEntry>(IReadOnlyCollection<ApiEnvelopMessage> message);

    Task<ApiEnvelopMessage> SendApiEnvelopMessagesAsync<TEntry>(ApiEnvelopMessage message);

    Task<IReadOnlyCollection<ApiEnvelopMessage>> ConsumeApiEnvelopMessagesAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken);

    Task AckMessagesAsync(IReadOnlyCollection<Guid> envelopMessagGuids, CancellationToken cancellationToken);
}
