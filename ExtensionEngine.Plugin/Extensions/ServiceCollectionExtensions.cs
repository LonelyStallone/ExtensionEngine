using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Abstractions.Plugins;
using ExtensionEngine.Plugin.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExtensionEngine.Plugin.InventoryManagement.MacOs.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPluginServices<TPlugin>(
        this IServiceCollection services,
        TPlugin plugin,
        IServiceProvider hostProvider)
        where TPlugin : IPlugin
    {
        var type = typeof(TPlugin);

        services.AddSingleton<IPluginInfoProvider>(plugin);
        services.AddSingleton<IPluginGateway, PluginGateway>(sp =>
        {
            var pluginInfoProvider = sp.GetRequiredService<IPluginInfoProvider>();
            var envelopeGateway = hostProvider.GetRequiredService<IEnvelopeGateway>();
            var messagePackagingService = sp.GetRequiredService<IMessagePackagingService>();

            return new PluginGateway(pluginInfoProvider, messagePackagingService, envelopeGateway);
        });

        
        var loggerFactory = hostProvider.GetRequiredService<ILoggerFactory>();
        services.AddScoped((_) => loggerFactory);

        return services;
    }
}
