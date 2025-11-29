using ExtensionEngine.Abstractions.Plugins;

namespace ExtensionEngine.Core.Storage.Abstractions;

public interface IPluginAssemblyLoader
{
    IPlugin LoadPluginFromAssembly(string assemblyPath);
}
