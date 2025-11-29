using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Plugin.InventoryManagement.MacOs.Extensions;
using ExtensionEngine.Plugin.InventoryManagement.MacOs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensionEngine.Plugin.InventoryManagement.MacOs;

public class TestInventoryPlugin : PluginBase
{
    public TestInventoryPlugin() : base()
    {
    }

    private Task _task = Task.CompletedTask;
    private CancellationTokenSource _source = new();

    public override Task StartAsync(IServiceProvider hostServiceProvider, CancellationToken cancellationToken)
    {
        using var scope = hostServiceProvider.CreateScope();
        var scopedServiceProvider = CeratePluginServiceProvider(scope.ServiceProvider);
        var loggerFactory = scopedServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<TestInventoryPlugin>();

        logger.LogInformation("StartAsync {Name}", Info.Name);

        _task = InternalStartAsync(hostServiceProvider);

        return _task;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _source.Cancel();

        return Task.CompletedTask;
    }

    public async Task InternalStartAsync(IServiceProvider hostServiceProvider)
    {
        using var scope = hostServiceProvider.CreateScope();
        var scopedServiceProvider = CeratePluginServiceProvider(scope.ServiceProvider);

        var gateway = scopedServiceProvider.GetRequiredService<IPluginGateway>();
        var loggerFactory = scopedServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<TestInventoryPlugin>();

        logger.LogInformation("{Name}", Info.Name);

        var token = _source.Token;

        try
        {
            var request = CreateProduceInventoryDataRequest();

            while (!token.IsCancellationRequested)
            {
                await gateway.PublishAsync(request, CancellationToken.None);
                logger.LogInformation("{Name}. INVENTORY PLUGIN. SUCCESS!", Info.Name);

                await Task.Delay(2000, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "INVENTORY PLUGIN REAL ERROR!");
        }
    }

    private UpdateInventoryEvent CreateProduceInventoryDataRequest()
    {
        var inventoryDataPluginVersion = new InventoryData
        {
            Key = $"PLUGIN_{nameof(Version)}",
            Value = Info.Version.ToString()
        };

        var inventoryDataPluginName = new InventoryData
        {
            Key = $"PLUGIN_{nameof(Info.Name)}",
            Value = Info.Name
        };

        var inventoryDataMachineName = new InventoryData
        {
            Key = $"DEVICE_{nameof(Environment.MachineName)}",
            Value = Environment.MachineName.ToString()
        };

        var inventoryData = new[] { inventoryDataPluginVersion, inventoryDataPluginName, inventoryDataMachineName };

        var request = new UpdateInventoryEvent
        {
            InventoryData = inventoryData
        };

        return request;
    }

    private IServiceProvider CeratePluginServiceProvider(IServiceProvider hostServiceProvider)
    {
        var services = new ServiceCollection();
        services.AddPluginServices<TestInventoryPlugin>();

        return services.BuildServiceProvider();
    }
}
