using ExtensionEngine.Abstractions.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace ExtensionEngine.Plugin.InventoryManagement.MacOs.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPluginServices<TPlugin>(
        this IServiceCollection services)
        where TPlugin : IPlugin
    {
        return services;
    }
}
