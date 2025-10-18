using ExtensionEngine.Plugin.Abstractions;

namespace ExtensionEngine.Core.Abstractions;

public interface IPluginRegistry
{
    Task<IReadOnlyCollection<IPluginContainer>> LoadAsync(IReadOnlyCollection<IPluginInfo> pluginMetadatas, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<IPluginInfo>> GetValidPluginVersions(CancellationToken cancellationToken);
}
