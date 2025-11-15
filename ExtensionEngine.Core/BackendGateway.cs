using ExtensionEngine.Plugin.Abstractions.Gateway;

namespace ExtensionEngine.Core;

public class BackendGateway : IPluginEndpointResolver
{
    public string SendMessageAsync()
    {
        return "http://localhost:5002";
    }
}
