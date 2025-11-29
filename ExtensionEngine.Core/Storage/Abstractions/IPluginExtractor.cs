using ExtensionEngine.Abstractions.Plugins;

namespace ExtensionEngine.Core.Storage.Abstractions;

public interface IPluginExtractor
{
    string ExtractPluginFromZip(IPluginContainer pluginContainer);
}
