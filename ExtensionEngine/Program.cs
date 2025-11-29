using ExtensionEngine;
using ExtensionEngine.Core.Management;
using PluginManagement.Proto;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddPluginManager();
        services.AddHostedService<PluginManager>();

        services.AddGrpcClient<PluginManagementFacade.PluginManagementFacadeClient>(options =>
        {
            options.Address = new Uri("https://localhost:5002");
        });

    })
    .Build();

await host.RunAsync();