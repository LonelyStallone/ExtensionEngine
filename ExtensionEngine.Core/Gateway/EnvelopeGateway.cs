using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Abstractions.Gateway.Models;
using ExtensionEngine.Abstractions.Plugins;

namespace ExtensionEngine.Core.Gateway;

public class EnvelopeGateway : IEnvelopeGateway
{
    public EnvelopeGateway()
    {
        
    }


    public Task AcksAsync(IPluginInfo pluginInfo, EnvelopMessage messages, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<EnvelopMessage> ConsumeAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task PublishAsync(IPluginInfo pluginInfo, EnvelopMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<EnvelopMessage> SendAsync(IPluginInfo pluginInfo, EnvelopMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
