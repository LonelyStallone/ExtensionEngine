using ExtensionEngine.Abstractions.Plugins;
using ExtensionEngine.Core.Management.Abstractions;

namespace ExtensionEngine.Core.Management;

public class GrpcPluginLoader : IPluginLoader
{
    private readonly string _hostId;

    public GrpcPluginLoader()
    {
        _hostId = Environment.MachineName;
    }

    public Task<IReadOnlyCollection<IPluginInfo>> GetValidPluginVersions(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<IPluginContainer>> LoadAsync(IReadOnlyCollection<IPluginInfo> pluginMetadatas, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // public async Task<IReadOnlyCollection<IPluginContainer>> LoadAsync(
    //     IReadOnlyCollection<IPluginInfo> pluginMetadatas,
    //     CancellationToken cancellationToken)
    // {
    //     // var request = new LoadPluginsRequest();
    //     // 
    //     // foreach (var metadata in pluginMetadatas)
    //     // {
    //     //     request.PluginInfo.Add(new PluginInfo
    //     //     {
    //     //         Name = metadata.Name,
    //     //         Version = metadata.Version.ToString(),
    //     //     });
    //     // }
    //     // 
    //     // using var channel = GrpcChannel.ForAddress("http://localhost:5002");
    //     // 
    //     // var client = new MizarManagementFacade.MizarManagementFacadeClient(channel);
    //     // var response = await client.LoadPluginsAsync(request, cancellationToken: cancellationToken);
    //     // 
    //     // var containers = new List<IPluginContainer>();
    //     // foreach (var containerProto in response.Container)
    //     // {
    //     //     var name = containerProto.PluginInfo.Name;
    //     //     var version = containerProto.PluginInfo.Version;
    //     //     var data = Convert.FromBase64String(containerProto.Data);
    //     // 
    //     //     containers.Add(new ExtensionEngine.Abstractions.Plugin.Models.PluginContainer(name, version, data));
    //     // }
    //     // 
    //     // return containers.AsReadOnly();
    // }
    // 
    // public async Task<IReadOnlyCollection<IPluginInfo>> GetValidPluginVersions(
    //     CancellationToken cancellationToken)
    // {
    //     // var request = new GetValidPluginVersionsRequest
    //     // {
    //     //     HostId = _hostId
    //     // };
    //     // 
    //     // using var channel = GrpcChannel.ForAddress("http://localhost:5002");
    //     // 
    //     // var client = new MizarManagementFacade.MizarManagementFacadeClient(channel);
    //     // var response = await client.GetValidPluginVersionsAsync(request, cancellationToken: cancellationToken);
    //     // 
    //     // var metadatas = new List<IPluginInfo>();
    //     // foreach (var metadataProto in response.PluginInfo)
    //     // {
    //     //     var name = metadataProto.Name;
    //     //     var version = metadataProto.Version;
    //     // 
    //     // 
    //     //     metadatas.Add(new ExtensionEngine.Abstractions.Plugin.Models.PluginInfo(name, version));
    //     // }
    //     // 
    //     // return metadatas.AsReadOnly();
    // }
}
