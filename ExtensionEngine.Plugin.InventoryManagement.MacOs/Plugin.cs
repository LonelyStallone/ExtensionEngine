using ExtensionEngine.Plugin.Abstractions;
using ExtensionEngine.Plugin.InventoryManagement.Proto;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensionEngine.Plugin.InventoryManagement.MacOs;

public class Plugin : IPlugin
{
    public string Version => typeof(Plugin).Assembly!.GetName().Version.ToString(3);

    public string Name => typeof(Plugin).Assembly!.GetName().Name;

    private Task _task = Task.CompletedTask;
    private CancellationTokenSource _source = new();

    public Task StartAsync(IServiceProvider hostServiceProvider, CancellationToken cancellationToken)
    {
        _task = (Version == "1.0.0") 
            ? InternalStartV0Async(hostServiceProvider)
            : InternalStartV1Async(hostServiceProvider);

        return _task;
    }

    public async Task InternalStartV0Async(IServiceProvider hostServiceProvider)
    {
        using var scope = hostServiceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;

        var pluginEndpointResolver = scopedServiceProvider.GetRequiredService<IPluginEndpointResolver>();
        var loggerFactory = scopedServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Plugin>();
        logger.LogInformation("{Name}", Name);

        var token = _source.Token;

        try
        {
            var url = pluginEndpointResolver.GetGatewayEndpoint();

            using var channel = GrpcChannel.ForAddress(url);
            var client = new InventoryPluginFacade.InventoryPluginFacadeClient(channel);

            var request = CreateProduceInventoryDataRequest();

            while (!token.IsCancellationRequested)
            {
                // var _ = await client.ProduceInventoryDataAsync(request, cancellationToken: CancellationToken.None);

                logger.LogError("{Name}. INVENTORY PLUGIN. ERROR!", Name);
                await Task.Delay(2000, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "INVENTORY PLUGIN REAL ERROR!");
        }
    }

    public async Task InternalStartV1Async(IServiceProvider hostServiceProvider)
    {
        using var scope = hostServiceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;

        var pluginEndpointResolver = scopedServiceProvider.GetRequiredService<IPluginEndpointResolver>();
        var loggerFactory = scopedServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Plugin>();
        logger.LogInformation("{Name}", Name);

        var token = _source.Token;

        try
        {
            var url = pluginEndpointResolver.GetGatewayEndpoint();

            using var channel = GrpcChannel.ForAddress(url);
            var client = new InventoryPluginFacade.InventoryPluginFacadeClient(channel);

            var request = CreateProduceInventoryDataRequest();

            while (!token.IsCancellationRequested)
            {
                var _ = await client.ProduceInventoryDataAsync(request, cancellationToken: CancellationToken.None);

                logger.LogInformation("{Name}. INVENTORY PLUGIN. SUCCESS!", Name);
                await Task.Delay(2000, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "INVENTORY PLUGIN REAL ERROR!");
        }
    }

    private ProduceInventoryDataRequest CreateProduceInventoryDataRequest()
    {
        var inventoryDataPluginVersion = new InventoryData
        {
            Key = $"PLUGIN_{nameof(Version)}",
            Value = Version.ToString()
        };

        var inventoryDataPluginName = new InventoryData
        {
            Key = $"PLUGIN_{nameof(Name)}",
            Value = Name
        };

        var inventoryDataMachineName = new InventoryData
        {
            Key = $"DEVICE_{nameof(Environment.MachineName)}",
            Value = Environment.MachineName.ToString()
        };

        var request = new ProduceInventoryDataRequest
        {
            InventoryData = { inventoryDataPluginVersion, inventoryDataPluginName, inventoryDataMachineName }
        };

        return request;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _source.Cancel();

        return Task.CompletedTask;
    }
}
