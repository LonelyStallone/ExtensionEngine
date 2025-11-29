using ExtensionEngine.Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<PluginManagementService>();
app.MapGrpcService<PluginGatewayService>();

app.Run();