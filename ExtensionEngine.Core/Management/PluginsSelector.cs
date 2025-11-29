using ExtensionEngine.Abstractions.Plugins;
using ExtensionEngine.Core.Management.Abstractions;
using Microsoft.Extensions.Logging;

namespace ExtensionEngine.Core.Management;

public class PluginsSelector : IPluginsSelector
{
    private readonly ILogger<PluginsSelector> _logger;

    public PluginsSelector(ILogger<PluginsSelector> logger)
    {
        _logger = logger;
    }

    public IReadOnlyCollection<IPluginInfo> GetMissingPlugins(
        IReadOnlyCollection<IPluginInfo> validPlugins,
        IReadOnlyCollection<IPluginInfo> currentPlugins)
    {
        var pluginsToLoad = new List<IPluginInfo>();

        foreach (var validPlugin in validPlugins)
        {
            var currentPlugin = currentPlugins.FirstOrDefault(
                p => p.Name == validPlugin.Name && p.Version == validPlugin.Version);

            if (currentPlugin == null)
            {
                pluginsToLoad.Add(validPlugin);
                _logger.LogDebug("Plugin {PluginName} v{Version} will be loaded - not found in storage",
                    validPlugin.Name, validPlugin.Version);
            }
            else
            {
                _logger.LogDebug("Plugin {PluginName} v{Version} already exists in storage",
                    validPlugin.Name, validPlugin.Version);
            }
        }

        return pluginsToLoad;
    }
}
