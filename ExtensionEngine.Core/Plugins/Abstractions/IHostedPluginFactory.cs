using ExtensionEngine.Plugin.Abstractions;

namespace ExtensionEngine.Core.Plugins.Abstractions;

public interface IHostedPluginFactory
{
    IHostedPlugin Create(IPluginInfo plugin);
}
