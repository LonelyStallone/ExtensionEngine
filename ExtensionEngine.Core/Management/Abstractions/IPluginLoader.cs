using ExtensionEngine.Abstractions.Plugins;

namespace ExtensionEngine.Core.Management.Abstractions;

public interface IPluginLoader
{
    Task<IReadOnlyCollection<IPluginContainer>> LoadAsync(IReadOnlyCollection<IPluginInfo> pluginMetadatas, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<IPluginInfo>> GetValidPluginVersions(CancellationToken cancellationToken);
}
