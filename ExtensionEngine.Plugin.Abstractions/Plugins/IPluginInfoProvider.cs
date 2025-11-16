using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Abstractions.Plugins;

public interface IPluginInfoProvider
{
    IPluginInfo GetPluginInfo();
}
