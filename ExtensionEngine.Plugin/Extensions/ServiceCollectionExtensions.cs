using ExtensionEngine.Abstractions.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace ExtensionEngine.Plugin.InventoryManagement.MacOs.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPluginServices<TPlugin>(
        this IServiceCollection services,
        IServiceProvider hostProvider)
        where TPlugin : IPlugin
    {
        var type = typeof(TPlugin);

        services.AddSingleton<IPluginInfoProvider, PluginInfoProvider>((_) => new PluginInfoProvider(type.Assembly));



        return services;
    }
}
