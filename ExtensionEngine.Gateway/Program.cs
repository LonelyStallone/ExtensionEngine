using ExtensionEngine.Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<MizarManagementService>();
app.MapGrpcService<MizarPluginsService>();

app.Run();