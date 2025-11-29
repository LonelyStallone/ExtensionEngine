using Grpc.Core;
using PluginManagement.Proto;

namespace ExtensionEngine.Gateway;

public class PluginManagementService : PluginManagementFacade.PluginManagementFacadeBase
{
    private readonly ILogger<PluginManagementService> _logger;
    private static Dictionary<string, List<PluginInfo>> ValidPluginVersions = new();

    public PluginManagementService(ILogger<PluginManagementService> logger)
    {
        _logger = logger;
    }

    public override async Task<GetValidPluginVersionsResponse> GetValidPluginVersions(GetValidPluginVersionsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Getting valid plugin versions for host: {HostId}", request.HostId);

        if (string.IsNullOrEmpty(request.HostId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Host ID is required"));
        }

        if (ValidPluginVersions.TryGetValue(request.HostId, out var pluginMetadata))
        {
            var response = new GetValidPluginVersionsResponse();
            response.PluginInfo.AddRange(pluginMetadata);
            return response;
        }

        // Если host_id не найден, возвращаем пустой список
        return new GetValidPluginVersionsResponse();
    }

    public override async Task<SetValidPluginVersionsResponse> SetValidPluginVersions(SetValidPluginVersionsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Setting valid plugin versions for host: {HostId}", request.HostId);

        if (string.IsNullOrEmpty(request.HostId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Host ID is required"));
        }

        if (request.PluginInfo == null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Plugin metadata is required"));
        }

        // Добавляем или обновляем метаданные плагина для указанного host_id
        if (!ValidPluginVersions.ContainsKey(request.HostId))
        {
            ValidPluginVersions[request.HostId] = new List<PluginInfo>();
        }

        // Проверяем, существует ли уже плагин с таким именем
        var existingPluginIndex = ValidPluginVersions[request.HostId]
            .FindIndex(p => p.Name == request.PluginInfo.Name);

        if (existingPluginIndex >= 0)
        {
            // Обновляем существующий плагин
            ValidPluginVersions[request.HostId][existingPluginIndex] = request.PluginInfo;
            _logger.LogInformation("Updated plugin {PluginName} for host {HostId}",
                request.PluginInfo.Name, request.HostId);
        }
        else
        {
            // Добавляем новый плагин
            ValidPluginVersions[request.HostId].Add(request.PluginInfo);
            _logger.LogInformation("Added new plugin {PluginName} for host {HostId}",
                request.PluginInfo.Name, request.HostId);
        }

        return new SetValidPluginVersionsResponse();
    }



    public override async Task<LoadPluginsResponse> LoadPlugins(LoadPluginsRequest request, ServerCallContext context)
    {
        var containers = new List<PluginContainer>();

        foreach (var metadata in request.PluginInfo)
        {
            var path = GetPluginPath(metadata);

            var data = await File.ReadAllBytesAsync(path);

            var container = new PluginContainer
            {
                Data = Convert.ToBase64String(data),
                PluginInfo = metadata
            };

            containers.Add(container);
        }

        return new LoadPluginsResponse
        {
            Container = { containers }
        };
    }

    private string GetPluginPath(PluginInfo pluginMetadata)
    {
        return $"Resources/{pluginMetadata.Name}_v{pluginMetadata.Version}.plugin";
    }
}