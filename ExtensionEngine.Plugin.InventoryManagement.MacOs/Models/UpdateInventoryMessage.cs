using ExtensionEngine.Abstractions.Gateway;
using System.Collections.Generic;

namespace ExtensionEngine.Plugin.InventoryManagement.MacOs.Models;

public class UpdateInventoryMessage : IMizarMessage
{
    public IReadOnlyCollection<InventoryData> InventoryData { get; init; }
}
