using ExtensionEngine.Core.Abstractions;
using ExtensionEngine.Core.Proto;
using ExtensionEngine.Plugin.Abstractions;
using Grpc.Net.Client;

namespace ExtensionEngine.Core;
public class GrpcPluginRegistry : IPluginRegistry
{
    private readonly string _hostId;
    private readonly IPluginEndpointResolver _pluginEndpointResolver;

    public GrpcPluginRegistry(IPluginEndpointResolver pluginEndpointResolver)
    {
        _hostId = Environment.MachineName;
        _pluginEndpointResolver = pluginEndpointResolver;
    }

    public async Task<IReadOnlyCollection<IPluginContainer>> LoadAsync(
        IReadOnlyCollection<IPluginInfo> pluginMetadatas,
        CancellationToken cancellationToken)
    {
        var request = new LoadPluginsRequest();

        foreach (var metadata in pluginMetadatas)
        {
            request.Metadata.Add(new PluginMetadata
            {
                Name = metadata.Name,
                Version = metadata.Version.ToString(),
            });
        }

        var url = _pluginEndpointResolver.GetGatewayEndpoint();
        using var channel = GrpcChannel.ForAddress(url);

        var client = new ExtensionEngineFacade.ExtensionEngineFacadeClient(channel);
        var response = await client.LoadPluginsAsync(request, cancellationToken: cancellationToken);

        var containers = new List<IPluginContainer>();
        foreach (var containerProto in response.Container)
        {
            var name = containerProto.Metadata.Name;
            var version = containerProto.Metadata.Version;
            var data = Convert.FromBase64String(containerProto.Data);

            containers.Add(new Plugin.Abstractions.Models.PluginContainer(name, version, data));
        }

        return containers.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<IPluginInfo>> GetValidPluginVersions(
        CancellationToken cancellationToken)
    {
        var request = new GetValidPluginVersionsRequest
        {
            HostId = _hostId
        };

        var url = _pluginEndpointResolver.GetGatewayEndpoint();
        using var channel = GrpcChannel.ForAddress(url);

        var client = new ExtensionEngineFacade.ExtensionEngineFacadeClient(channel);
        var response = await client.GetValidPluginVersionsAsync(request, cancellationToken: cancellationToken);

        var metadatas = new List<IPluginInfo>();
        foreach (var metadataProto in response.Metadata)
        {
            var name = metadataProto.Name;
            var version =metadataProto.Version;


            metadatas.Add(new Plugin.Abstractions.Models.PluginInfo(name, version));
        }

        return metadatas.AsReadOnly();
    }
}
