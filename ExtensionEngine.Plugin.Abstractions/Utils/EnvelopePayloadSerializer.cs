using ExtensionEngine.Abstractions.Gateway;
using System.Text.Json;

namespace ExtensionEngine.Abstractions.Utils;

public class EnvelopePayloadSerializer
{
    public static IMessage Deserialize(string type, string data)
    {
        try
        {
            var messageType = Type.GetType(type);

            if (messageType == null)
            {
                throw new ArgumentException($"Тип '{type}' не найден");
            }

            var result = JsonSerializer.Deserialize(data, messageType);

            if (result is not IMessage message)
            {
                throw new InvalidOperationException("Invalid deserialize");
            }

            return message;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Ошибка десериализации JSON: {ex.Message}", ex);
        }
    }

    public static TMessage Deserialize<TMessage>(string data)
    {
        try
        {
            var messageType = typeof(TMessage);

            var result = JsonSerializer.Deserialize<TMessage>(data);

            if (result is not IMessage)
            {
                throw new InvalidOperationException("Invalid deserialize");
            }

            // Приводим к целевому типу
            return result;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Ошибка десериализации JSON: {ex.Message}", ex);
        }
    }

    public static string Serialize<TMessage>(TMessage message)
    {
        try
        {
            return JsonSerializer.Serialize(message);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Ошибка сериализации: {ex.Message}", ex);
        }
    }
}
