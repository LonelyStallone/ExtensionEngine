using ExtensionEngine.Core.Abstractions;
using ExtensionEngine.Plugin.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

public class RuntimePluginTracker : IRuntimePluginTracker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RuntimePluginTracker> _logger;

    private readonly ConcurrentDictionary<string, IPlugin> _activePlugins = new();

    public RuntimePluginTracker(
        IServiceProvider serviceProvider,
        ILogger<RuntimePluginTracker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task AddAndStartAsync(IPlugin plugin, CancellationToken cancellationToken)
    {
        var pluginKey = plugin.Name;

        if (_activePlugins.TryGetValue(pluginKey, out var runningPlugin))
        {
            // Плагин уже запущен - проверяем версию
            if (runningPlugin.Version == plugin.Version)
            {
                _logger.LogInformation("Plugin {PluginName} v{Version} is already running", pluginKey, plugin.Version);
                return;
            }

            // Версия изменилась - останавливаем старую и запускаем новую
            _logger.LogInformation("Plugin {PluginName} version changed from v{OldVersion} to v{NewVersion}, updating...",
                pluginKey,
                runningPlugin.Version,
                plugin.Version);

            await StopPluginSafeAsync(runningPlugin, cancellationToken);
            _activePlugins.TryRemove(pluginKey, out _);
        }

        // Запускаем новый плагин
        if (_activePlugins.TryAdd(pluginKey, plugin))
        {
            plugin.StartAsync(_serviceProvider, cancellationToken);
            _logger.LogInformation("Plugin {PluginName} v{Version} started successfully", pluginKey, plugin.Version);
        }
    }

    public IReadOnlyCollection<IPluginInfo> GetActivePluginMetadata()
    {
        return _activePlugins.Values.ToList().AsReadOnly();
    }

    public async Task StopAndRemoveAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken)
    {
        var pluginKey = pluginInfo.Name;

        if (_activePlugins.TryRemove(pluginKey, out var plugin))
        {
            try
            {
                _logger.LogInformation("Stopping plugin {PluginName} version {Version}", plugin.Name, plugin.Version);

                await plugin.StopAsync(cancellationToken);

                _logger.LogInformation("Plugin {PluginName} version {Version} stopped successfully", plugin.Name, plugin.Version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop plugin {PluginName} version {Version}", plugin.Name, plugin.Version);
                throw;
            }
        }
        else
        {
            _logger.LogWarning("Plugin {PluginName} version {Version} not found in active plugins", pluginInfo.Name, pluginInfo.Version);
        }
    }

    public bool TryGetVersion(string pluginName, Version version)
    {
        if (string.IsNullOrEmpty(pluginName))
            throw new ArgumentException("Plugin name cannot be null or empty", nameof(pluginName));

        return _activePlugins.ContainsKey(pluginName);
    }

    private async Task StopPluginSafeAsync(IPlugin plugin, CancellationToken cancellationToken)
    {
        try
        {
            await plugin.StopAsync(cancellationToken);

            _logger.LogInformation("Plugin {PluginName} v{Version} stopped successfully", plugin.Name, plugin.Version);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop plugin {PluginName} v{Version}", plugin.Name, plugin.Version);
        }
    }
}