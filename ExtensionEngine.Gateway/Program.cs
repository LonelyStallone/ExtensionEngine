using ExtensionEngine.Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<ExtensionEngineFacadeService>();
app.MapGrpcService<InventoryPluginFacadeService>();

app.Run();