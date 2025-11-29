using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Abstractions.Gateway.Models;
using ExtensionEngine.Abstractions.Plugins;
using ExtensionEngine.Plugin.Abstractions;

namespace ExtensionEngine.Plugin;

public class PluginGateway : IPluginGateway
{
    private readonly IPluginInfoProvider _pluginInfoProvider;
    private readonly IMessagePackagingService _messagePackagingService;
    private readonly IEnvelopeGateway _envelopeGateway;

    public PluginGateway(
        IPluginInfoProvider pluginInfoProvider,
        IMessagePackagingService messagePackagingService,
        IEnvelopeGateway envelopeGateway)
    {
        _pluginInfoProvider = pluginInfoProvider;
        _messagePackagingService = messagePackagingService;
        _envelopeGateway = envelopeGateway;
    }

    public Task AckAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var pluginInfo = _pluginInfoProvider.Info;
        var ackMessageReq = new AckMessageRequest
        {
            MessageId = messageId,
        };

        var envelopMessageRequest = _messagePackagingService.Pack(ackMessageReq);

        return _envelopeGateway.AcksAsync(pluginInfo, envelopMessageRequest, cancellationToken);
    }

    public async Task<IMessageResponse> ConsumeAsync(CancellationToken cancellationToken)
    {
        var pluginInfo = _pluginInfoProvider.Info;
        var envelopMessageResponse = await _envelopeGateway.ConsumeAsync(pluginInfo, cancellationToken);

        var message = _messagePackagingService.Unpack(envelopMessageResponse);

        if (message is not IMessageResponse messageResponse)
        {
            throw new InvalidCastException("Invalid cast consumed message."); 
        }

        return messageResponse;
    }

    public Task PublishAsync<TMessageRequest>(TMessageRequest messageRequest, CancellationToken cancellationToken)
        where TMessageRequest : IMessageRequest
    {
        var pluginInfo = _pluginInfoProvider.Info;
        var envelopMessageRequest = _messagePackagingService.Pack(messageRequest);

        return _envelopeGateway.PublishAsync(pluginInfo, envelopMessageRequest, cancellationToken);
    }

    public async Task<TMessageResponse> SendAsync<TMessageRequest, TMessageResponse>(TMessageRequest messageRequest, CancellationToken cancellationToken)
        where TMessageRequest : IMessageRequest
        where TMessageResponse : IMessageResponse
    {
        var pluginInfo = _pluginInfoProvider.Info;
        var envelopMessageRequest = _messagePackagingService.Pack(messageRequest);

        var envelopMessageResponse = await _envelopeGateway.SendAsync(pluginInfo, envelopMessageRequest, cancellationToken);

        var messageResponse = _messagePackagingService.Unpack<TMessageResponse>(envelopMessageResponse);

        return messageResponse;
    }
}
