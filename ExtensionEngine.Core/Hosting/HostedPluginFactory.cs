using Microsoft.Extensions.DependencyInjection;
using ExtensionEngine.Core.Plugins.Abstractions;
using Microsoft.Extensions.Logging;
using ExtensionEngine.Core.Storage.Abstractions;
using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Core.Plugins;

public class HostedPluginFactory : IHostedPluginFactory
{
    private readonly IServiceProvider _serviceProvider;

    public HostedPluginFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IHostedPlugin Create(IPluginInfo pluginInfo)
    {
        var serviceProvider = _serviceProvider;
        var logger = _serviceProvider.GetRequiredService<ILogger<HostedPlugin>>();
        var pluginContainerStorage = _serviceProvider.GetRequiredService<IPluginContainerStorage>();
        var pluginExtractor = _serviceProvider.GetRequiredService<IPluginExtractor>();
        var pluginAssemblyLoader = _serviceProvider.GetRequiredService<IPluginAssemblyLoader>();

        return new HostedPlugin(
            pluginInfo,
            serviceProvider,
            logger,
            pluginContainerStorage,
            pluginExtractor,
            pluginAssemblyLoader);
    }
}