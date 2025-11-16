using ExtensionEngine.Abstractions.Gateway;
using ExtensionEngine.Abstractions.Plugin;
using ExtensionEngine.Plugin.InventoryManagement.MacOs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExtensionEngine.Plugin.InventoryManagement.MacOs;

public class TestInventoryPlugin : IPlugin
{
    public string Version => typeof(TestInventoryPlugin).Assembly!.GetName().Version.ToString(3);

    public string Name => typeof(TestInventoryPlugin).Assembly!.GetName().Name;

    private Task _task = Task.CompletedTask;
    private CancellationTokenSource _source = new();

    public Task StartAsync(IServiceProvider hostServiceProvider, CancellationToken cancellationToken)
    {
        _task = InternalStartAsync(hostServiceProvider);

        return _task;
    }

    public async Task InternalStartAsync(IServiceProvider hostServiceProvider)
    {
        using var scope = hostServiceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;

        var gateway = scopedServiceProvider.GetRequiredService<IMizarGateway>();
        var loggerFactory = scopedServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<TestInventoryPlugin>();
        logger.LogInformation("{Name}", Name);

        var token = _source.Token;

        try
        {
            var request = CreateProduceInventoryDataRequest();

            while (!token.IsCancellationRequested)
            {

                await gateway.ProduceMessagesAsync([request], CancellationToken.None);
                logger.LogInformation("{Name}. INVENTORY PLUGIN. SUCCESS!", Name);

                await Task.Delay(2000, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "INVENTORY PLUGIN REAL ERROR!");
        }
    }

    private UpdateInventoryMessage CreateProduceInventoryDataRequest()
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

        var inventoryData = new[] { inventoryDataPluginVersion, inventoryDataPluginName, inventoryDataMachineName };

        var request = new UpdateInventoryMessage
        {
            InventoryData = inventoryData
        };

        return request;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _source.Cancel();

        return Task.CompletedTask;
    }

    private void CeratePluginServiceProvider(IServiceProvider hostServiceProvider)
    {
        // var services = new ServiceCollection();
        // 
        // // Регистрация сервисов
        // services.AddTransient<IEmailService, SmtpEmailService>();
        // services.AddScoped<IUserService, UserService>();
        // 
        // // Построение Service Provider
        // return services.BuildServiceProvider();
        // 
        // // Использование сервисов
        // using (var scope = serviceProvider.CreateScope())
        // {
        //     var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        //     userService.RegisterUser("user@example.com", "John Doe");
        // }
        // 
        // var gateway = scopedServiceProvider.GetRequiredService<IMizarGateway>();
        // var loggerFactory = scopedServiceProvider.GetRequiredService<ILoggerFactory>();
    }
}
