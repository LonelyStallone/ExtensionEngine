using ExtensionEngine.Plugin.Abstractions;

namespace ExtensionEngine.Core.Abstractions;

public interface IMissingPluginsSelector
{
    IReadOnlyCollection<IPluginInfo> GetMissingPlugins(
        IReadOnlyCollection<IPluginInfo> validPlugins,
        IReadOnlyCollection<IPluginInfo> currentPlugins);
}
