using ExtensionEngine.Plugin.Abstractions;

namespace ExtensionEngine.Core.Abstractions;

public interface IRuntimePluginTracker
{
    Task AddAndStartAsync(IPlugin plugin, CancellationToken cancellationToken);

    Task StopAndRemoveAsync(IPluginInfo pluginMetadata, CancellationToken cancellationToken);

    bool TryGetVersion(string pluginaName, Version version);

    public IReadOnlyCollection<IPluginInfo> GetActivePluginMetadata();
}
