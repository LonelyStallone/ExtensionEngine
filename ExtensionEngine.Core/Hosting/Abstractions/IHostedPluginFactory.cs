using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Core.Plugins.Abstractions;

public interface IHostedPluginFactory
{
    IHostedPlugin Create(IPluginInfo plugin);
}
