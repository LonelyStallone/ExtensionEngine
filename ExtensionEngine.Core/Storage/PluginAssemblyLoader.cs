using ExtensionEngine.Abstractions.Plugins;
using ExtensionEngine.Core.Storage.Abstractions;
using System.Reflection;
using System.Runtime.Loader;

public class PluginAssemblyLoader : IPluginAssemblyLoader
{
    public IPlugin LoadPluginFromAssembly(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
            throw new FileNotFoundException($"Сборка не найдена: {assemblyPath}");

        // Создаем временный контекст загрузки
        var loadContext = new PluginLoadContext(assemblyPath);

        try
        {
            // Загружаем сборку через контекст
            var assembly = loadContext.LoadFromAssemblyPath(assemblyPath);

            // Ищем тип плагина
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) &&
                                   !t.IsInterface && !t.IsAbstract);

            if (pluginType == null)
                throw new InvalidOperationException($"В сборке {assemblyPath} не найден тип, реализующий IPlugin");

            // Создаем экземпляр плагина
            var plugin = Activator.CreateInstance(pluginType) as IPlugin;
            if (plugin == null)
                throw new InvalidOperationException($"Тип {pluginType.Name} не может быть создан");

            return plugin;
        }
        finally
        {
            // Выгружаем контекст (в .NET Core 3.0+)
            loadContext.Unload();
        }
    }

    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Сначала host, потом плагин
            return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName)
                ?? LoadFromResolver(assemblyName);
        }

        private Assembly LoadFromResolver(AssemblyName assemblyName)
        {
            var path = _resolver.ResolveAssemblyToPath(assemblyName);
            return path != null ? LoadFromAssemblyPath(path) : null;
        }
    }
}
