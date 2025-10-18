namespace ExtensionEngine.Plugin.Abstractions;

public interface IPlugin : IPluginInfo
{
    Task StartAsync(IServiceProvider hostServiceProvider, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
