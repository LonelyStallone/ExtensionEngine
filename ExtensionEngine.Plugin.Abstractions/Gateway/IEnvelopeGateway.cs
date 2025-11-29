using ExtensionEngine.Abstractions.Gateway.Models;
using ExtensionEngine.Abstractions.Plugins;

namespace ExtensionEngine.Abstractions.Gateway;
 
public interface IEnvelopeGateway
{
    Task PublishAsync(IPluginInfo pluginInfo, EnvelopMessage message, CancellationToken cancellationToken);

    Task<EnvelopMessage> SendAsync(IPluginInfo pluginInfo, EnvelopMessage message, CancellationToken cancellationToken);

    Task<EnvelopMessage> ConsumeAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken);

    Task AcksAsync(IPluginInfo pluginInfo, EnvelopMessage messages, CancellationToken cancellationToken);
}
