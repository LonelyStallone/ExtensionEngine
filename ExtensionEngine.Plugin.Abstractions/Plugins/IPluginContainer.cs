namespace ExtensionEngine.Abstractions.Plugin;

public interface IPluginContainer : IPluginInfo
{
    byte[] Data { get; }
}
