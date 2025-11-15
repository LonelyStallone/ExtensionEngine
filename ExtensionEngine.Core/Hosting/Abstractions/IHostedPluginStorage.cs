using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Core.Hosting.Abstractions;

public interface IHostedPluginStorage
{
    Task AddOrUpdateAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken);

    Task StopAndRemoveAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken);

    public IReadOnlyCollection<IPluginInfo> GetActivePluginInfo();
}
