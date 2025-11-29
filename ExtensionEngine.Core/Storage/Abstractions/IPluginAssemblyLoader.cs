using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Core.Storage.Abstractions;

public interface IPluginAssemblyLoader
{
    IPlugin LoadPluginFromAssembly(string assemblyPath);
}
