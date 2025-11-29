using ExtensionEngine.Abstractions.Plugins;
using ExtensionEngine.Abstractions.Plugins.Models;

namespace ExtensionEngine.Plugin;

public abstract class PluginBase : IPlugin
{
    private IPluginInfo? _info;

    public abstract Task StartAsync(IServiceProvider hostServiceProvider, CancellationToken cancellationToken);

    public abstract Task StopAsync(CancellationToken cancellationToken);

    public IPluginInfo Info => _info ??= GetInfo();

    public IPluginInfo GetInfo()
    {
        var pluginAssembly = this.GetType().Assembly;

        var name = pluginAssembly.GetName().Name
            ?? throw new ArgumentNullException($"Invalid assembly name. Assembly: {pluginAssembly.FullName}");

        var version = pluginAssembly.GetName().Version?.ToString(3)
            ?? throw new ArgumentNullException($"Invalid assembly version. Assembly: {name}");

        return new PluginInfo(name, version);
    }
}
