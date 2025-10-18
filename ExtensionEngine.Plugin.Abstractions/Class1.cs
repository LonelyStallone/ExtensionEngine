using System.Reflection;

namespace ExtensionEngine.Plugin.Abstractions;

public static class AssemblyHelper
{
    public static string GetName(Assembly assembly) => assembly.GetName().Name;

    public static Version GetVersion() =>
        Assembly.GetExecutingAssembly().GetName().Version;
}
