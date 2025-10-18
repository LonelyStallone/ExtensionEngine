namespace ExtensionEngine.Plugin.Abstractions;

public interface IPluginContainer : IPluginInfo
{
    byte[] Data { get; }
}
