using ExtensionEngine.Core.Abstractions;
using ExtensionEngine.Plugin.Abstractions;
using Microsoft.Extensions.Logging;

namespace ExtensionEngine.Core;

public class MissingPluginsSelector : IMissingPluginsSelector
{
    private readonly ILogger<MissingPluginsSelector> _logger;

    public MissingPluginsSelector(ILogger<MissingPluginsSelector> logger)
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
