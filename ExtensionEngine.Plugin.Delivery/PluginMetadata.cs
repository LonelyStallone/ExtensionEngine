namespace ExtensionEngine.Plugin.Delivery;

public class PluginMetadata
{
    public string PluginName { get; set; }
    public string Version { get; set; }
    public string MainAssembly { get; set; }
    public string EntryType { get; set; }
    public DateTime Created { get; set; }
    public int AssemblyCount { get; set; }
    public int ResourceCount { get; set; }
}