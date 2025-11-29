using ExtensionEngine.Abstractions.Plugins;
using ExtensionEngine.Abstractions.Plugins.Models;
using System.Reflection;

namespace ExtensionEngine.Plugin;

public class PluginInfoProvider : IPluginInfoProvider
{
    public PluginInfoProvider(Assembly assembly)
    {
        var pluginAssembly = assembly;

        var name = pluginAssembly.GetName().Name
            ?? throw new ArgumentNullException($"Invalid assembly name. Assembly: {pluginAssembly.FullName}");

        var version = pluginAssembly.GetName().Version?.ToString(3)
            ?? throw new ArgumentNullException($"Invalid assembly version. Assembly: {name}");

        PluginInfo = new PluginInfo(name, version);
    }

    public IPluginInfo PluginInfo { get; }
}
