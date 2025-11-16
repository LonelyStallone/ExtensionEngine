using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace ExtensionEngine.Core.Storage;

public class PluginZipExtractor
{
    private readonly string _tempExtractPath = "TempExtracted";

    private ILogger<PluginZipExtractor> _logger;

    public PluginZipExtractor(ILogger<PluginZipExtractor> logger)
    {
        _logger = logger;
    }

    public string ExtractPluginFromZip(byte[] zipData, string pluginName)
    {
        CreateDirectoryIfNotExist(_tempExtractPath);

        // Создаем временную директорию для распаковки
        var extractPath = Path.Combine(_tempExtractPath, Guid.NewGuid().ToString());
        CreateDirectoryIfNotExist(extractPath);

        try
        {
            // Распаковываем ZIP
            ExtractZipToDirectory(zipData, extractPath);

            // Проверяем наличие DLL файлов
            var dllFiles = Directory.GetFiles(extractPath + @"\assemblies", "*.dll");
            if (dllFiles.Length == 0)
                throw new FileNotFoundException("В ZIP архиве не найдены DLL файлы");

            // Ищем основную сборку плагина
            var assemblyDllName = $"{pluginName}.dll";
            var mainAssemblyPath = dllFiles.Single(fileName =>
                fileName.EndsWith(assemblyDllName, StringComparison.OrdinalIgnoreCase));

            return mainAssemblyPath;
        }
        catch
        {
            // В случае ошибки очищаем временные файлы
            CleanupDirectory(extractPath);
            throw;
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

    private static void ExtractZipToDirectory(byte[] zipData, string targetDirectory)
    {
        using var memoryStream = new MemoryStream(zipData);
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        archive.ExtractToDirectory(targetDirectory, true);
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

    private static string ToAbsolutePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));

        // Если путь уже абсолютный, возвращаем как есть
        if (Path.IsPathRooted(path))
            return Path.GetFullPath(path); // Нормализует путь

        // Для относительных путей - комбинируем с текущей рабочей директорией
        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
    }

    private void CreateDirectoryIfNotExist(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
        }
        catch
        {
            _logger.LogWarning("Create folder error: {path}", path);
        }
    }
}
