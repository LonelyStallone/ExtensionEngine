using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Core;
using ExtensionEngine.Core.Hosting.Abstractions;
using ExtensionEngine.Core.Management;
using ExtensionEngine.Core.Management.Abstractions;
using ExtensionEngine.Core.Storage;
using ExtensionEngine.Core.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ExtensionEngine;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPluginManager(this IServiceCollection services)
    {
        services.AddSingleton<IPluginManager, PluginManager>();

        services.AddSingleton<IBackendGateway, PluginEndpointResolver>();
        services.AddSingleton<IHostedPluginStorage, HostedPluginStorage>();
        services.AddSingleton<IPluginLoader, GrpcPluginLoader>();
        services.AddSingleton<IPluginContainerStorage, PluginContainerStorage>();
        services.AddSingleton<IPluginsSelector, MissingPluginsSelector>();
        services.AddSingleton<IPluginFactory, PluginContainerExtractor>();

        return services;
    }
}
