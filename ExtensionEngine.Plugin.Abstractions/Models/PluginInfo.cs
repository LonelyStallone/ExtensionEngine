namespace ExtensionEngine.Plugin.Abstractions.Models;

public class PluginInfo : IPluginInfo
{
    public PluginInfo(string name, string version)
    {
        Name = name;
        Version = version;
    }

    public string Name { get; }
    public string Version { get; }
}
