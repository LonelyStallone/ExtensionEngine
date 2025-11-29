namespace ExtensionEngine.Abstractions.Gateway;

public interface IPluginGateway
{
    Task PublishAsync<TMessageRequest>(TMessageRequest messageRequest, CancellationToken cancellationToken)
        where TMessageRequest : IMessageRequest;

    Task<TMessageResponse> SendAsync<TMessageRequest, TMessageResponse>(TMessageRequest messageRequest, CancellationToken cancellationToken)
        where TMessageRequest : IMessageRequest
        where TMessageResponse : IMessageResponse;

    Task<IMessageResponse> ConsumeAsync(CancellationToken cancellationToken);

    Task AckAsync(Guid messageId, CancellationToken cancellationToken);
}
