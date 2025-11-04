using Microsoft.Extensions.DependencyInjection;
using ExtensionEngine.Core.Abstractions;
using ExtensionEngine.Plugin.Abstractions;
using ExtensionEngine.Core.Plugins.Abstractions;
using Microsoft.Extensions.Logging;

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
        var pluginFactory = _serviceProvider.GetRequiredService<IPluginFactory>();

        return new HostedPlugin(pluginInfo, serviceProvider, logger, pluginContainerStorage, pluginFactory);
    }
}