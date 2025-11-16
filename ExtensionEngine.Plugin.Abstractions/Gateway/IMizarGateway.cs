namespace ExtensionEngine.Abstractions.Gateway;

public interface IMizarGateway
{
    Task ProduceMessagesAsync<TMessage>(IReadOnlyCollection<TMessage> message, CancellationToken cancellationToken)
        where TMessage : IMizarMessage;

    Task<IMizarMessage> SendMessageAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
        where TMessage : IMizarMessage;

    Task<IReadOnlyCollection<IMizarMessage>> ConsumeMessagesAsync<TMessage>(CancellationToken cancellationToken)
        where TMessage : IMizarMessage;

    Task AckMessagesAsync(IReadOnlyCollection<Guid> envelopMessagGuids, CancellationToken cancellationToken);
}
