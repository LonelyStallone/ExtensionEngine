namespace ExtensionEngine.Plugin.Abstractions.Extensions;

public static class PluginInfoExtensions
{
    public static bool IsEqualsMetadata(this IPluginInfo firstMetadata, IPluginInfo secondMetadata)
    {
        return firstMetadata.Name == secondMetadata.Name && firstMetadata.Version == secondMetadata.Version;
    }

    public static string GetDescription(this IPluginInfo pluginMetadata)
    {
        return $"Plugin: {pluginMetadata.Name} v{pluginMetadata.Version}".ToLower();
    }
}
