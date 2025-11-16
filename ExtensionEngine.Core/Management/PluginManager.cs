using ExtensionEngine.Abstractions.Plugin;
using ExtensionEngine.Core.Hosting.Abstractions;
using ExtensionEngine.Core.Management.Abstractions;
using ExtensionEngine.Core.Storage.Abstractions;
using ExtensionEngine.Plugin.Abstractions.Extensions;
using Microsoft.Extensions.Logging;

namespace ExtensionEngine.Core.Management;

public class PluginManager : IPluginManager
{
    private const int UpdateDelaySeconds = 10;

    private readonly IHostedPluginStorage _runtimePluginTracker;
    private readonly IPluginLoader _pluginLoader;
    private readonly IPluginContainerStorage _pluginContainerStorage;
    private readonly IPluginsSelector _missingPluginsSelector;
    private readonly ILogger<PluginManager> _logger;

    public PluginManager(
        IHostedPluginStorage runtimePluginTracker,
        IPluginLoader pluginLoader,
        IPluginContainerStorage pluginContainerStorage,
        IPluginsSelector missingPluginsSelector,
        ILogger<PluginManager> logger)
    {
        _runtimePluginTracker = runtimePluginTracker;
        _pluginLoader = pluginLoader;
        _pluginContainerStorage = pluginContainerStorage;
        _missingPluginsSelector = missingPluginsSelector;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting PluginManager...");

            while (true)
            {
                await UpdatePluginsAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(UpdateDelaySeconds));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start PluginManager");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task UpdatePluginsAsync(CancellationToken cancellationToken)
    {
        // Получаем список валидных версий плагинов c бэка
        var validPluginMetadatas = await _pluginLoader.GetValidPluginVersions(cancellationToken);
        _logger.LogInformation("Found {Count} valid plugins", validPluginMetadatas.Count);

        // Получаем текущие плагины из хранилища плагинов
        var currentPluginMetadatas = await _pluginContainerStorage.GetPluginsAsync(cancellationToken);
        _logger.LogInformation("Found {Count} plugins in storage", currentPluginMetadatas.Count);

        // Определяем, какие плагины нужно загрузить с бэка
        var pluginsMetadataToLoad = _missingPluginsSelector.GetMissingPlugins(validPluginMetadatas, currentPluginMetadatas);
        _logger.LogInformation("Need load {Count} plugins", pluginsMetadataToLoad.Count);

        // Загрузжаем недостающие плагины с бэка
        await LoadMissingPluginsAsync(pluginsMetadataToLoad, cancellationToken);

        // Активируем плагины
        await AddOrUpdatePluginsAsync(validPluginMetadatas, cancellationToken);
    }

    private async Task LoadMissingPluginsAsync(IReadOnlyCollection<IPluginInfo> pluginsToLoad, CancellationToken cancellationToken)
    {
        if (pluginsToLoad.Any())
        {
            _logger.LogInformation("Loading {Count} new/updated plugins", pluginsToLoad.Count);

            var loadedPluginContainers = await _pluginLoader.LoadAsync(pluginsToLoad, cancellationToken);
            await _pluginContainerStorage.AddPluginsAsync(loadedPluginContainers, cancellationToken);

            _logger.LogInformation("Successfully loaded {Count} plugins", loadedPluginContainers.Count);
        }
    }

    private async Task AddOrUpdatePluginsAsync(IReadOnlyCollection<IPluginInfo> pluginMetadatasToUpdate, CancellationToken cancellationToken)
    {
        foreach (var pluginMetadata in pluginMetadatasToUpdate)
        {
            _logger.LogInformation("Runing plugin {Plugin} from local storage", pluginMetadata.GetDescription());
            await _runtimePluginTracker.AddOrUpdateAsync(pluginMetadata, cancellationToken);
        }
    }
}