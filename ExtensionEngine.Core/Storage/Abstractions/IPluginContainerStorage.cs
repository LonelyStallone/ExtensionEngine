using ExtensionEngine.Abstractions.Plugins;

namespace ExtensionEngine.Core.Storage.Abstractions;

public interface IPluginContainerStorage
{
    Task<IReadOnlyCollection<IPluginInfo>> GetPluginsAsync(CancellationToken cancellationToken);

    Task<IPluginContainer> GetPluginContainerAsync(IPluginInfo plugin, CancellationToken cancellationToken);

    Task AddPluginsAsync(IReadOnlyCollection<IPluginContainer> pluginContainers, CancellationToken cancellationToken);
}
