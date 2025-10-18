using ExtensionEngine.Core.Abstractions;
using ExtensionEngine.Plugin.Abstractions;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;

public class PluginFactory : IPluginFactory
{
    private readonly string _tempExtractPath = "TempExtracted";

    public PluginFactory()
    {
        Directory.CreateDirectory(_tempExtractPath);
    }

    public IPlugin Create(IPluginContainer pluginContainer)
    {
        var zipData = pluginContainer.Data;
        var assemblyDllName = $"{pluginContainer.Name}.dll";

        return LoadPluginFromZip(zipData, assemblyDllName);
    }

    private IPlugin LoadPluginFromZip(byte[] zipData, string assemblyDllName)
    {
        CreateTempDirectoryIfNotExists();
        // Создаем временную директорию для распаковки
        var extractPath = Path.Combine(_tempExtractPath, Guid.NewGuid().ToString());
        Directory.CreateDirectory(extractPath);

        try
        {
            // Распаковываем ZIP
            ExtractZipToDirectory(zipData, extractPath);

            // Ищем первую DLL как основную сборку
            var dllFiles = Directory.GetFiles(extractPath + @"\assemblies", "*.dll");
            if (dllFiles.Length == 0)
                throw new FileNotFoundException("В ZIP архиве не найдены DLL файлы");

            var mainAssemblyPath = dllFiles.Single(fileName => fileName.EndsWith(assemblyDllName, StringComparison.OrdinalIgnoreCase)); // Берем первую DLL

            // Загружаем плагин из сборки
            return LoadPluginFromAssembly(mainAssemblyPath);
        }
        catch
        {
            // В случае ошибки очищаем временные файлы
            CleanupDirectory(extractPath);
            throw;
        }
    }

    private IPlugin LoadPluginFromAssemblyOld(string assemblyPath)
    {
        if (!File.Exists(assemblyPath))
            throw new FileNotFoundException($"Сборка не найдена: {assemblyPath}");

        // Загружаем сборку напрямую
        var assembly = Assembly.LoadFrom(assemblyPath);

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

    private IPlugin LoadPluginFromAssembly(string assemblyPath)
    {
        var absolutPath = ToAbsolutePath(assemblyPath);
        if (!File.Exists(absolutPath))
            throw new FileNotFoundException($"Сборка не найдена: {absolutPath}");

        // Создаем временный контекст загрузки
        var loadContext = new PluginLoadContext(absolutPath);

        try
        {
            // Загружаем сборку через контекст
            var assembly = loadContext.LoadFromAssemblyPath(absolutPath);

            // Ищем тип плагина
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) &&
                                   !t.IsInterface && !t.IsAbstract);

            if (pluginType == null)
                throw new InvalidOperationException($"В сборке {absolutPath} не найден тип, реализующий IPlugin");

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

    public void Dispose()
    {
        // Очищаем все временные директории
        try
        {
            Directory.Delete(_tempExtractPath, true);
        }
        catch
        {
            // Игнорируем ошибки очистки
        }
    }

    private void CreateTempDirectoryIfNotExists()
    {
        try
        {
            Directory.CreateDirectory(_tempExtractPath);
        }
        catch
        {
            // Игнорируем ошибки создания
        }
    }

    private static void ExtractZipToDirectory(byte[] zipData, string targetDirectory)
    {
        using var memoryStream = new MemoryStream(zipData);
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        archive.ExtractToDirectory(targetDirectory, true);
    }

    private static byte[] CreateZipFromDirectory(string directory, string mainAssembly, CompressionLevel compressionLevel)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            // Добавляем все DLL файлы из директории
            foreach (var file in Directory.GetFiles(directory, "*.dll"))
            {
                var entryName = Path.GetFileName(file);
                archive.CreateEntryFromFile(file, entryName, compressionLevel);
            }
        }

        return memoryStream.ToArray();
    }

    private static void CleanupDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
        catch
        {
            // Игнорируем ошибки очистки
        }
    }

    public static string ToAbsolutePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));

        // Если путь уже абсолютный, возвращаем как есть
        if (Path.IsPathRooted(path))
            return Path.GetFullPath(path); // Нормализует путь

        // Для относительных путей - комбинируем с текущей рабочей директорией
        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
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