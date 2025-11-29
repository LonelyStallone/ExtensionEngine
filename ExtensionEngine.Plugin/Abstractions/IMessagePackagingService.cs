using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Abstractions.Gateway.Models;

namespace ExtensionEngine.Plugin.Abstractions;

public interface IMessagePackagingService
{
    EnvelopMessage Pack<TMessage>(TMessage message)
        where TMessage : IMessage;

    TMessage Unpack<TMessage>(EnvelopMessage message)
        where TMessage : IMessage;

    IMessage Unpack(EnvelopMessage message);
}
