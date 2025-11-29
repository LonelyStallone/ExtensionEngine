namespace ExtensionEngine.Abstractions.Plugins.Models;

public class PluginContainer : IPluginContainer
{
    public PluginContainer(string name, string version, byte[] data)
    {
        Name = name;
        Version = version;
        Data = data;
    }

    public string Name { get; }

    public string Version { get; }

    public byte[] Data { get; }
}
