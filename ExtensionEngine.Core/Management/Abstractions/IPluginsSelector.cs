using ExtensionEngine.Abstractions.Plugins;

namespace ExtensionEngine.Core.Management.Abstractions;

public interface IPluginsSelector
{
    IReadOnlyCollection<IPluginInfo> GetMissingPlugins(
        IReadOnlyCollection<IPluginInfo> validPlugins,
        IReadOnlyCollection<IPluginInfo> currentPlugins);
}
