using ExtensionEngine.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionEngine.Core;
public class PluginEndpointResolver : IPluginEndpointResolver
{
    public string GetGatewayEndpoint()
    {
        return "http://localhost:5002";
    }
}
