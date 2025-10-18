using ExtensionEngine.Plugin.Abstractions;

namespace ExtensionEngine.Core.Abstractions;

public interface IPluginContainerStorage
{
    Task<IReadOnlyCollection<IPluginInfo>> GetPluginsAsync(CancellationToken cancellationToken);

    Task<IPluginContainer> GetPluginContainerAsync(IPluginInfo plugin, CancellationToken cancellationToken);

    Task AddPluginsAsync(IReadOnlyCollection<IPluginContainer> pluginContainers, CancellationToken cancellationToken);
}
