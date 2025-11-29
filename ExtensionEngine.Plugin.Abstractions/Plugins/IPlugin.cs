namespace ExtensionEngine.Abstractions.Plugins;

public interface IPlugin : IPluginInfoProvider
{
    Task StartAsync(IServiceProvider hostServiceProvider, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
