using ExtensionEngine;
using ExtensionEngine.Core.Management;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddPluginManager();
        services.AddHostedService<PluginManager>();

        services.AddGrpcClient<ExtensionEngineFacade.ExtensionEngineFacadeClient>(options =>
        {
            options.Address = new Uri("https://localhost:5002");
        });

    })
    .Build();

await host.RunAsync();