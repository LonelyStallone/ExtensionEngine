using ExtensionEngine.Abstractions.Gateway;
using MediatR;
using System.Collections.Generic;

namespace ExtensionEngine.Plugin.InventoryManagement.MacOs.Models;

public class UpdateInventoryEvent : IMessageRequest, INotification
{
    public IReadOnlyCollection<InventoryData> InventoryData { get; init; }
}
