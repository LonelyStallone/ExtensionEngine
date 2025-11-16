using ExtensionEngine.Abstractions.Gateway.Models;
using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Abstractions.Gateway;

public interface IMizarEnvelopeGateway
{
    Task ProduceApiEnvelopMessagesAsync(IReadOnlyCollection<ApiEnvelopMessage> message);

    Task<ApiEnvelopMessage> SendApiEnvelopMessageAsync(ApiEnvelopMessage message);

    Task<IReadOnlyCollection<ApiEnvelopMessage>> ConsumeApiEnvelopMessagesAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken);

    Task AckApiEnvelopMessagesAsync(IReadOnlyCollection<Guid> envelopMessagGuids, CancellationToken cancellationToken);
}
