using ExtensionEngine.Plugin.InventoryManagement;
using ExtensionEngine.Plugin.InventoryManagement.Proto;
using Grpc.Core;

namespace ExtensionEngine.Gateway;

public class InventoryPluginFacadeService : InventoryPluginFacade.InventoryPluginFacadeBase
{
    private readonly ILogger<InventoryPluginFacadeService> _logger;

    public InventoryPluginFacadeService(ILogger<InventoryPluginFacadeService> logger)
    {
        _logger = logger;
    }

    public override Task<ProduceInventoryDataResponse> ProduceInventoryData(ProduceInventoryDataRequest request, ServerCallContext context)
    {
        // Логируем общую информацию о запросе
        _logger.LogInformation("Received ProduceInventoryData request with {Count} inventory items", request.InventoryData.Count);

        // Логируем детальную информацию о каждом элементе InventoryData
        foreach (var inventoryItem in request.InventoryData)
        {
            _logger.LogInformation("Inventory Item - Key: {Key}, Value: {Value}", inventoryItem.Key, inventoryItem.Value);
        }

        // Возвращаем пустой ответ
        return Task.FromResult(new ProduceInventoryDataResponse());
    }
}