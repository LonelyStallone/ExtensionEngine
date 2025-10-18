using ExtensionEngine.Core;
using ExtensionEngine.Core.Abstractions;
using ExtensionEngine.Plugin.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ExtensionEngine;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPluginManager(this IServiceCollection services)
    {
        services.AddSingleton<IPluginManager, PluginManager>();

        services.AddSingleton<IPluginEndpointResolver, PluginEndpointResolver>();
        services.AddSingleton<IRuntimePluginTracker, RuntimePluginTracker>();
        services.AddSingleton<IPluginRegistry, GrpcPluginRegistry>();
        services.AddSingleton<IPluginContainerStorage, FilePluginContainerStorage>();
        services.AddSingleton<IMissingPluginsSelector, MissingPluginsSelector>();
        services.AddSingleton<IPluginFactory, PluginFactory>();

        return services;
    }
}
