using ExtensionEngine.Abstractions.Plugin;
using System.IO.Compression;
using System.Reflection;

public class PluginLoader : IDisposable
{
    private readonly string _tempExtractPath;

    public PluginLoader(string tempPath = "TempExtracted")
    {
        _tempExtractPath = tempPath;
        Directory.CreateDirectory(_tempExtractPath);
    }

    public IPlugin LoadPluginFromAssembly(string assemblyPath)
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

    public IPlugin LoadPluginFromZip(byte[] zipData)
    {
        // Создаем временную директорию для распаковки
        var extractPath = Path.Combine(_tempExtractPath, Guid.NewGuid().ToString());
        Directory.CreateDirectory(extractPath);

        try
        {
            // Распаковываем ZIP
            ExtractZipToDirectory(zipData, extractPath);

            // Ищем первую DLL как основную сборку
            var dllFiles = Directory.GetFiles(extractPath, "*.dll");
            if (dllFiles.Length == 0)
                throw new FileNotFoundException("В ZIP архиве не найдены DLL файлы");

            var mainAssemblyPath = dllFiles[0]; // Берем первую DLL

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

    public IPlugin LoadPluginFromZipFile(string zipFilePath)
    {
        var zipData = File.ReadAllBytes(zipFilePath);
        return LoadPluginFromZip(zipData);
    }

    public byte[] CreatePluginZip(string assemblyPath, CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        if (!File.Exists(assemblyPath))
            throw new FileNotFoundException($"Сборка не найдена: {assemblyPath}");

        var directory = Path.GetDirectoryName(assemblyPath);
        var mainAssembly = Path.GetFileName(assemblyPath);

        return CreateZipFromDirectory(directory, mainAssembly, compressionLevel);
    }

    public void SavePluginToZipFile(string assemblyPath, string outputZipPath, CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        var zipData = CreatePluginZip(assemblyPath, compressionLevel);
        File.WriteAllBytes(outputZipPath, zipData);
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

    public void Dispose()
    {
        // Очищаем все временные директории
        try
        {
            if (Directory.Exists(_tempExtractPath))
                Directory.Delete(_tempExtractPath, true);
        }
        catch
        {
            // Игнорируем ошибки очистки
        }
    }
}