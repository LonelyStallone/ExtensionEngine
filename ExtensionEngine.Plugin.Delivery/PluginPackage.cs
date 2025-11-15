using ExtensionEngine.Abstractions.Plugin;
using System.Reflection;

namespace ExtensionEngine.Plugin.Delivery;

[Serializable]
public class PluginPackage
{
    public string PluginName { get; set; }
    public string Version { get; set; }
    public string MainAssembly { get; set; }
    public string EntryType { get; set; }
    public Dictionary<string, byte[]> Assemblies { get; set; } = new();
    public Dictionary<string, byte[]> Resources { get; set; } = new();
    public string Configuration { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public static PluginPackage CreateFromFile(string pluginAbsolutePath)
    {
        var package = new PluginPackage();
        var pluginDirectory = Path.GetDirectoryName(pluginAbsolutePath);
        var mainAssemblyName = Path.GetFileName(pluginAbsolutePath);

        if (!File.Exists(pluginAbsolutePath))
            throw new FileNotFoundException($"Основная сборка не найдена: {pluginAbsolutePath}");

        // Загружаем основную сборку для получения метаданных
        var assembly = Assembly.LoadFrom(pluginAbsolutePath);
        var pluginType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        if (pluginType == null)
            throw new InvalidOperationException($"В сборке {mainAssemblyName} не найден тип, реализующий IPlugin");

        var pluginInstance = Activator.CreateInstance(pluginType) as IPlugin;
        if (pluginInstance is null)
            throw new InvalidOperationException("Плагин не собрался");

        package.PluginName = pluginInstance.Name ?? Path.GetFileNameWithoutExtension(mainAssemblyName);
        package.Version = pluginInstance.Version.ToString();
        package.MainAssembly = mainAssemblyName;
        package.EntryType = pluginType.FullName;

        // Собираем все сборки из директории
        foreach (var file in Directory.GetFiles(pluginDirectory, "*.*", SearchOption.AllDirectories))
        {
            var extension = Path.GetExtension(file).ToLower();
            var fileName = Path.GetFileName(file);

            if (extension == ".dll" || extension == ".exe")
            {
                package.Assemblies[fileName] = File.ReadAllBytes(file);
            }
            else if (extension == ".json" || extension == ".config" || extension == ".xml")
            {
                package.Resources[fileName] = File.ReadAllBytes(file);
            }
        }

        return package;
    }

    public void SaveToDirectory(string targetDirectory)
    {
        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        // Сохраняем сборки
        foreach (var assembly in Assemblies)
        {
            var filePath = Path.Combine(targetDirectory, assembly.Key);
            File.WriteAllBytes(filePath, assembly.Value);
        }

        // Сохраняем ресурсы
        foreach (var resource in Resources)
        {
            var filePath = Path.Combine(targetDirectory, resource.Key);
            File.WriteAllBytes(filePath, resource.Value);
        }

        // Сохраняем конфигурацию
        if (!string.IsNullOrEmpty(Configuration))
        {
            File.WriteAllText(Path.Combine(targetDirectory, "plugin.config.json"), Configuration);
        }
    }
}