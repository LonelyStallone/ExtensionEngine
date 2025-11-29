using ExtensionEngine.Abstractions.Plugins;

namespace ExtensionEngine.Core.Plugins.Abstractions;

public interface IHostedPluginFactory
{
    IHostedPlugin Create(IPluginInfo plugin);
}
