using ExtensionEngine.Plugin.Abstractions;

namespace ExtensionEngine.Core.Abstractions;

public interface IPluginFactory
{
    IPlugin Create(IPluginContainer pluginContainer);
}
