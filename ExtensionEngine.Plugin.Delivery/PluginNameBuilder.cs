namespace ExtensionEngine.Plugin.Delivery;

public static class PluginNameBuilder
{
    public static string GetPluginFileName(string name, string version)
    {
        var safeName = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
        return $"{safeName}_v{version}.plugin";
    }
}
