using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Abstractions.Gateway.Models;
using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Core.Gateway;

public class BackendGateway : IBackendGateway
{
    public Task AckMessagesAsync(IReadOnlyCollection<Guid> envelopMessagGuids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<ApiEnvelopMessage>> ConsumeApiEnvelopMessagesAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProduceApiEnvelopMessagesAsync(IReadOnlyCollection<ApiEnvelopMessage> message)
    {
        throw new NotImplementedException();
    }

    public Task<ApiEnvelopMessage> SendApiEnvelopMessagesAsync(ApiEnvelopMessage message)
    {
        throw new NotImplementedException();
    }
}
