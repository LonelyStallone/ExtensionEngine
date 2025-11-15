using ExtensionEngine.Abstractions.Plugin;

namespace ExtensionEngine.Core.Storage.Abstractions;

public interface IPluginFactory
{
    IPlugin LoadPluginFromAssembly(string assemblyPath);
}
