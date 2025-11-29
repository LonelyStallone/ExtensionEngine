using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Core.Gateway;
using ExtensionEngine.Core.Hosting.Abstractions;
using ExtensionEngine.Core.Management;
using ExtensionEngine.Core.Management.Abstractions;
using ExtensionEngine.Core.Storage;
using ExtensionEngine.Core.Storage.Abstractions;

namespace ExtensionEngine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPluginManager(this IServiceCollection services)
    {
        services.AddSingleton<IPluginManager, PluginManager>();

        services.AddScoped<IEnvelopeGateway, EnvelopeGateway>();

        services.AddSingleton<IHostedPluginStorage, HostedPluginStorage>();

        services.AddSingleton<IPluginLoader, GrpcPluginLoader>();
        services.AddSingleton<IPluginContainerStorage, PluginContainerStorage>();
        services.AddSingleton<IPluginsSelector, PluginsSelector>();
        services.AddSingleton<IPluginAssemblyLoader, PluginAssemblyLoader>();
        services.AddSingleton<IPluginContainerStorage, PluginContainerStorage>();
        services.AddSingleton<IPluginExtractor, PluginZipExtractor>();

        return services;
    }
}
