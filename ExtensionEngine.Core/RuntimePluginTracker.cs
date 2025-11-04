using ExtensionEngine.Core.Abstractions;
using ExtensionEngine.Core.Plugins.Abstractions;
using ExtensionEngine.Plugin.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

public class RuntimePluginTracker : IRuntimePluginTracker
{
    private readonly IHostedPluginFactory _hostedPluginFactory;
    private readonly ILogger<RuntimePluginTracker> _logger;

    private readonly ConcurrentDictionary<string, IHostedPlugin> _activePlugins = new();

    public RuntimePluginTracker(
        IHostedPluginFactory hostedPluginFactory,
        ILogger<RuntimePluginTracker> logger)
    {
        _hostedPluginFactory = hostedPluginFactory;
        _logger = logger;
    }

    public async Task AddOrUpdateAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken)
    {
        var pluginKey = pluginInfo.Name;

        if (_activePlugins.TryGetValue(pluginKey, out var runningPlugin))
        {
            runningPlugin.SetActualVersion(pluginInfo);

            return;
        }

        _logger.LogInformation("Creating new hosted plugin for {PluginName}", pluginKey);
        var hostedPlugin = _hostedPluginFactory.Create(pluginInfo);

        if (_activePlugins.TryAdd(pluginKey, hostedPlugin))
        {
            await hostedPlugin.StartAsync(cancellationToken);

            _logger.LogInformation("Plugin {PluginName} v{Version} started successfully", pluginKey, hostedPlugin.Version);
        }
        else
        {
            _logger.LogWarning("Failed to add plugin {PluginName} to active plugins dictionary - it may have been added by another thread", pluginKey);
        }
    }

    public async Task StopAndRemoveAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken)
    {
        var pluginKey = pluginInfo.Name;

        if (_activePlugins.TryRemove(pluginKey, out var hostedPlugin))
        {
            try
            {
                await hostedPlugin.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop plugin {PluginName}", pluginKey);
                throw;
            }
        }
        else
        {
            _logger.LogWarning("Plugin {PluginName} not found in active plugins", pluginKey);
        }
    }

    public IReadOnlyCollection<IPluginInfo> GetActivePluginInfo()
    {
        return _activePlugins.Values.ToList().AsReadOnly();
    }
}