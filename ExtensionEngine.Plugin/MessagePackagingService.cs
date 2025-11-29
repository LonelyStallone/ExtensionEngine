using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Abstractions.Gateway.Models;
using ExtensionEngine.Abstractions.Utils;

namespace ExtensionEngine.Plugin.Abstractions;

public class MessagePackagingService : IMessagePackagingService
{
    public EnvelopMessage Pack<TMessage>(TMessage message)
        where TMessage : IMessage
    {
        var payload = EnvelopePayloadSerializer.Serialize(message);

        return new EnvelopMessage
        {
            Info = new MessageInfo
            {
                CreatedAt = DateTime.UtcNow,
                SpanId = Guid.NewGuid(),
                TraceId = Guid.NewGuid()
            },
            Payload = new MessagePayload
            {
                Type = typeof(TMessage),
                Data = payload
            }
        };
    }

    public TMessage Unpack<TMessage>(EnvelopMessage envelopMessage)
        where TMessage : IMessage
    {
        var payloadData = envelopMessage.Payload.Data;
        var message = EnvelopePayloadSerializer.Deserialize<TMessage>(payloadData);

        return message;
    }

    public IMessage Unpack(EnvelopMessage envelopMessage)
    {
        var typeFullName = envelopMessage.Payload.Type.FullName!;
        var payloadData = envelopMessage.Payload.Data;
        var message = EnvelopePayloadSerializer.Deserialize(typeFullName, payloadData);

        return message;
    }
    
}