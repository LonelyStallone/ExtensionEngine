using ExtensionEngine.Plugin.Abstractions;

namespace ExtensionEngine.Core.Plugins.Abstractions;
public interface IPluginConfigurationService
{
    TimeSpan GetUpdatePluginInterval(IPluginInfo pluginInfo);
}
