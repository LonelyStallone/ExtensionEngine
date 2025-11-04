using ExtensionEngine.Plugin.Abstractions;

namespace ExtensionEngine.Core.Abstractions;

public interface IRuntimePluginTracker
{
    Task AddOrUpdateAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken);

    Task StopAndRemoveAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken);

    public IReadOnlyCollection<IPluginInfo> GetActivePluginInfo();
}
