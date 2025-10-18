using ExtensionEngine.Core.Abstractions;
using ExtensionEngine.Plugin.Abstractions;
using ExtensionEngine.Plugin.Abstractions.Extensions;
using Microsoft.Extensions.Logging;

namespace ExtensionEngine.Core;

public class PluginManager : IPluginManager
{
    private const int UpdateDelaySeconds = 10;

    private readonly IRuntimePluginTracker _runtimePluginTracker;
    private readonly IPluginRegistry _pluginRegistry;
    private readonly IPluginContainerStorage _pluginContainerStorage;
    private readonly IMissingPluginsSelector _missingPluginsSelector;
    private readonly IPluginFactory _pluginFactory;
    private readonly ILogger<PluginManager> _logger;

    public PluginManager(
        IRuntimePluginTracker runtimePluginTracker,
        IPluginRegistry pluginRegistry,
        IPluginContainerStorage pluginContainerStorage,
        IMissingPluginsSelector missingPluginsSelector,
        IPluginFactory pluginFactory,
        ILogger<PluginManager> logger)
    {
        _runtimePluginTracker = runtimePluginTracker;
        _pluginRegistry = pluginRegistry;
        _pluginContainerStorage = pluginContainerStorage;
        _missingPluginsSelector = missingPluginsSelector;
        _pluginFactory = pluginFactory;
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
        var validPluginMetadatas = await _pluginRegistry.GetValidPluginVersions(cancellationToken);
        _logger.LogInformation("Found {Count} valid plugins", validPluginMetadatas.Count);

        // Получаем текущие плагины из хранилища плагинов
        var currentPluginMetadatas = await _pluginContainerStorage.GetPluginsAsync(cancellationToken);
        _logger.LogInformation("Found {Count} plugins in storage", currentPluginMetadatas.Count);

        // Определяем, какие плагины нужно загрузить с бэка
        var pluginsMetadataToLoad = _missingPluginsSelector.GetMissingPlugins(validPluginMetadatas, currentPluginMetadatas);
        _logger.LogInformation("Need load {Count} plugins", pluginsMetadataToLoad.Count);

        // Загрузжаем недостающие плагины с бэка
        await LoadMissingPluginsAsync(pluginsMetadataToLoad, cancellationToken);

        // Получаем список активных плагинов из трекера активных плагинов
        var activePluginMetadatas = _runtimePluginTracker.GetActivePluginMetadata();
        _logger.LogInformation("Active {Count} plugins", activePluginMetadatas.Count);

        // Получаем список плагинов для обновления
        var pluginMetadatasToUpdate = validPluginMetadatas
            .Where(validPluginMetadata => !activePluginMetadatas.Any(activePluginMetadata => validPluginMetadata.IsEqualsMetadata(activePluginMetadata)))
            .ToList();

        _logger.LogInformation("Need activate {Count} plugins", pluginMetadatasToUpdate.Count);

        // Активируем плагины
        await AddAndRunPluginsAsync(pluginMetadatasToUpdate, cancellationToken);
    }

    private async Task LoadMissingPluginsAsync(IReadOnlyCollection<IPluginInfo> pluginsToLoad, CancellationToken cancellationToken)
    {
        if (pluginsToLoad.Any())
        {
            _logger.LogInformation("Loading {Count} new/updated plugins", pluginsToLoad.Count);

            var loadedPluginContainers = await _pluginRegistry.LoadAsync(pluginsToLoad, cancellationToken);
            await _pluginContainerStorage.AddPluginsAsync(loadedPluginContainers, cancellationToken);

            _logger.LogInformation("Successfully loaded {Count} plugins", loadedPluginContainers.Count);
        }
    }

    private async Task AddAndRunPluginsAsync(IReadOnlyCollection<IPluginInfo> pluginMetadatasToUpdate, CancellationToken cancellationToken)
    {
        foreach (var pluginMetadata in pluginMetadatasToUpdate)
        {
            var plugin = await GetPluginAsync(pluginMetadata, cancellationToken);

            _logger.LogInformation("Runing plugin {Plugin} from local storage", plugin.GetDescription());
            await _runtimePluginTracker.AddAndStartAsync(plugin, cancellationToken);
        }
    }

    private async Task<IPlugin> GetPluginAsync(IPluginInfo pluginMetadata, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading plugin container {Plugin} from local storage", pluginMetadata.GetDescription());
        var pluginContainers = await _pluginContainerStorage.GetPluginContainerAsync(pluginMetadata, cancellationToken);

        _logger.LogInformation("Convert container {Plugin} to .NET plugin", pluginMetadata.GetDescription());
        var plugin = _pluginFactory.Create(pluginContainers);

        return plugin;
    }
}