namespace ExtensionEngine.Plugin.Delivery;

public class PluginDeliveryInfo
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string MainAssembly { get; set; }
    public string EntryType { get; set; }
    public DateTime Created { get; set; }
    public int AssemblyCount { get; set; }
    public int ResourceCount { get; set; }
    public long ZipSize { get; set; }
    public double CompressionRatio => ZipSize > 0 ? (double)AssemblyCount * 1024 * 1024 / ZipSize : 0;
}