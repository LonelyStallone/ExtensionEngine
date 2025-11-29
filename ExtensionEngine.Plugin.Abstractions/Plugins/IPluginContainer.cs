namespace ExtensionEngine.Abstractions.Plugins;

public interface IPluginContainer : IPluginInfo
{
    byte[] Data { get; }
}
