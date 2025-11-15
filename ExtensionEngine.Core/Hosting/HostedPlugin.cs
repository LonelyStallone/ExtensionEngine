using Microsoft.Extensions.Logging;
using ExtensionEngine.Plugin.Abstractions.Extensions;
using ExtensionEngine.Core.Plugins.Abstractions;
using ExtensionEngine.Core.Storage.Abstractions;
using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Core.Plugins;

public class HostedPlugin : IHostedPlugin
{ 
    private static readonly TimeSpan UpdateIntervalSeconds = TimeSpan.FromSeconds(1);

    private IPluginInfo _actualPluginVersion;
    private IPlugin _plugin;

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HostedPlugin> _logger;
    private readonly IPluginContainerStorage _pluginContainerStorage;
    private readonly IPluginFactory _pluginFactory;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private Task? _backgroundTask;

    public HostedPlugin(
        IPluginInfo actualPluginVersion,
        IServiceProvider serviceProvider,
        ILogger<HostedPlugin> logger,
        IPluginContainerStorage pluginContainerStorage,
        IPluginFactory pluginFactory)
    {
        _actualPluginVersion = actualPluginVersion;

        _serviceProvider = serviceProvider;
        _logger = logger;
        _pluginContainerStorage = pluginContainerStorage;
        _pluginFactory = pluginFactory;
    }

    public string Version => _actualPluginVersion.Version;

    public string Name => _actualPluginVersion.Name;

    public void SetActualVersion(IPluginInfo pluginInfo)
    {
        ThrowIfPluginInfoNotValid(pluginInfo);

        _actualPluginVersion = pluginInfo;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_backgroundTask != null)
        {
            _logger.LogWarning("HostedPlugin {PluginName} is already started", Name);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Starting HostedPlugin {PluginName}", Name);
        _backgroundTask = RunAsync();

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_backgroundTask is null)
            return;

        _logger.LogInformation("Disposing HostedPlugin {PluginName}", Name);

        try
        {
            _cancellationTokenSource.Cancel();

            await _backgroundTask;

            await StopPluginAsync(_plugin, cancellationToken);

            _cancellationTokenSource.Dispose();
            _backgroundTask = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during HostedPlugin disposal");
        }
    }

    private async Task RunAsync()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                if (!NeedStartPlugin(_actualPluginVersion, _plugin))
                {
                    await Task.Delay(UpdateIntervalSeconds);
                    continue;
                }

                await StopPluginAsync(_plugin, _cancellationTokenSource.Token);

                _plugin = await GetActualPluginAsync(_actualPluginVersion, _cancellationTokenSource.Token);

                await StartPluginAsync(_plugin, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) when (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error during plugin execution");
                await Task.Delay(UpdateIntervalSeconds);
            }
        }
    }

    private bool NeedStartPlugin(IPluginInfo actualPluginVersion, IPlugin plugin)
    {
        if (actualPluginVersion is null)
        {
            _logger.LogDebug("NeedStartPlugin: returning false - actual plugin version is null");
            return false;
        }

        if (!actualPluginVersion.Version.Equals(plugin.Version, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug(
                "NeedStartPlugin: returning true - plugin version changed from {CurrentVersion} to {NewVersion}",
                actualPluginVersion.Version,
                plugin?.Version ?? "Undefined");

            return true;
        }

        _logger.LogDebug("NeedStartPlugin: returning false - plugin is already running with current version");
        return false;
    }

    private async Task StopPluginAsync(IPlugin plugin, CancellationToken cancellationToken)
    {
        if (plugin is not null)
        {
            _logger.LogInformation("Stopping previous version of plugin {PluginName}", Name);

            await plugin.StopAsync(_cancellationTokenSource.Token);

            _logger.LogInformation("Plugin {PluginName} version {Version} stopped successfully", Name, Version);
        }
        else
        {
            _logger.LogInformation("Plugin {PluginName} not initialize. Not need stopped.", Name);
        }
    }

    private async Task<IPlugin> GetActualPluginAsync(IPluginInfo pluginInfo, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading plugin container {Plugin} from local storage", pluginInfo.GetDescription());
        var pluginContainers = await _pluginContainerStorage.GetPluginContainerAsync(pluginInfo, cancellationToken);

        _logger.LogInformation("Convert container {Plugin} to .NET plugin", pluginInfo.GetDescription());
        var plugin = _pluginFactory.Create(pluginContainers);

        return plugin;
    }

    private async Task StartPluginAsync(IPlugin plugin, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting plugin {PluginName} version {Version}", Name, Version);

        await _plugin.StartAsync(_serviceProvider, cancellationToken);

        _logger.LogInformation("Plugin {PluginName} version {Version} started successfully", Name, Version);
    }

    private void ThrowIfPluginInfoNotValid(IPluginInfo pluginInfo)
    {
        if (pluginInfo == null)
            throw new ArgumentNullException(nameof(pluginInfo));

        if (string.IsNullOrWhiteSpace(pluginInfo.Name))
            throw new ArgumentException("Plugin name cannot be null or empty", nameof(pluginInfo));

        if (string.IsNullOrWhiteSpace(pluginInfo.Version))
            throw new ArgumentException("Plugin version cannot be null or empty", nameof(pluginInfo));

        if (!pluginInfo.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(Name))
        {
            throw new ArgumentException("Plugin cannot be rewritten. Names are not equal.", nameof(pluginInfo));
        }
    }
}
