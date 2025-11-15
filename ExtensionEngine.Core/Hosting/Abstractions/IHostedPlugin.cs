using ExtensionEngine.Abstractions.Plugin;
using Microsoft.Extensions.Hosting;

namespace ExtensionEngine.Core.Plugins.Abstractions;

public interface IHostedPlugin : IPluginInfo, IHostedService
{
    void SetActualVersion(IPluginInfo pluginInfo);
}
