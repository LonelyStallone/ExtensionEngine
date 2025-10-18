using System.IO.Compression;
using System.Text.Json;

namespace ExtensionEngine.Plugin.Delivery;
public static class PluginZipSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static byte[] SerializeToZip(PluginPackage package, CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            // Сериализуем метаданные плагина
            var metadataEntry = archive.CreateEntry("metadata.json", compressionLevel);
            using (var metadataStream = metadataEntry.Open())
            using (var metadataWriter = new StreamWriter(metadataStream))
            {
                var metadata = new
                {
                    package.PluginName,
                    package.Version,
                    package.MainAssembly,
                    package.EntryType,
                    package.Created,
                    AssemblyCount = package.Assemblies.Count,
                    ResourceCount = package.Resources.Count
                };
                var metadataJson = JsonSerializer.Serialize(metadata, JsonOptions);
                metadataWriter.Write(metadataJson);
            }

            // Сохраняем конфигурацию
            if (!string.IsNullOrEmpty(package.Configuration))
            {
                var configEntry = archive.CreateEntry("configuration.json", compressionLevel);
                using var configStream = configEntry.Open();
                using var configWriter = new StreamWriter(configStream);
                configWriter.Write(package.Configuration);
            }

            // Сохраняем сборки в папку assemblies/
            foreach (var assembly in package.Assemblies)
            {
                var assemblyEntry = archive.CreateEntry($"assemblies/{assembly.Key}", compressionLevel);
                using var assemblyStream = assemblyEntry.Open();
                assemblyStream.Write(assembly.Value, 0, assembly.Value.Length);
            }

            // Сохраняем ресурсы в папку resources/
            foreach (var resource in package.Resources)
            {
                var resourceEntry = archive.CreateEntry($"resources/{resource.Key}", compressionLevel);
                using var resourceStream = resourceEntry.Open();
                resourceStream.Write(resource.Value, 0, resource.Value.Length);
            }

            // Создаем манифест
            var manifestEntry = archive.CreateEntry("manifest.txt", compressionLevel);
            using (var manifestStream = manifestEntry.Open())
            using (var manifestWriter = new StreamWriter(manifestStream))
            {
                manifestWriter.WriteLine($"Plugin: {package.PluginName}");
                manifestWriter.WriteLine($"Version: {package.Version}");
                manifestWriter.WriteLine($"Main Assembly: {package.MainAssembly}");
                manifestWriter.WriteLine($"Entry Type: {package.EntryType}");
                manifestWriter.WriteLine($"Created: {package.Created:yyyy-MM-dd HH:mm:ss}");
                manifestWriter.WriteLine($"Assemblies: {package.Assemblies.Count}");
                manifestWriter.WriteLine($"Resources: {package.Resources.Count}");
            }
        }

        return memoryStream.ToArray();
    }

    public static PluginPackage DeserializeFromZip(byte[] zipData)
    {
        var package = new PluginPackage();

        using var memoryStream = new MemoryStream(zipData);
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        // Читаем метаданные
        var metadataEntry = archive.GetEntry("metadata.json");
        if (metadataEntry == null)
            throw new InvalidDataException("ZIP архив не содержит metadata.json");

        using (var metadataStream = metadataEntry.Open())
        using (var metadataReader = new StreamReader(metadataStream))
        {
            var metadataJson = metadataReader.ReadToEnd();
            var metadata = JsonSerializer.Deserialize<PluginMetadata>(metadataJson, JsonOptions);

            package.PluginName = metadata.PluginName;
            package.Version = metadata.Version;
            package.MainAssembly = metadata.MainAssembly;
            package.EntryType = metadata.EntryType;
            package.Created = metadata.Created;
        }

        // Читаем конфигурацию
        var configEntry = archive.GetEntry("configuration.json");
        if (configEntry != null)
        {
            using var configStream = configEntry.Open();
            using var configReader = new StreamReader(configStream);
            package.Configuration = configReader.ReadToEnd();
        }

        // Читаем сборки
        var assemblyEntries = archive.Entries
            .Where(e => e.FullName.StartsWith("assemblies/") && e.Length > 0)
            .ToList();

        foreach (var entry in assemblyEntries)
        {
            var assemblyName = Path.GetFileName(entry.FullName);
            using var assemblyStream = entry.Open();
            using var ms = new MemoryStream();
            assemblyStream.CopyTo(ms);
            package.Assemblies[assemblyName] = ms.ToArray();
        }

        // Читаем ресурсы
        var resourceEntries = archive.Entries
            .Where(e => e.FullName.StartsWith("resources/") && e.Length > 0)
            .ToList();

        foreach (var entry in resourceEntries)
        {
            var resourceName = Path.GetFileName(entry.FullName);
            using var resourceStream = entry.Open();
            using var ms = new MemoryStream();
            resourceStream.CopyTo(ms);
            package.Resources[resourceName] = ms.ToArray();
        }

        return package;
    }

    public static async Task<byte[]> SerializeToZipAsync(PluginPackage package, CompressionLevel compressionLevel = CompressionLevel.SmallestSize)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            // Асинхронная сериализация метаданных
            var metadataEntry = archive.CreateEntry("metadata.json", compressionLevel);
            await using (var metadataStream = metadataEntry.Open())
            await using (var metadataWriter = new StreamWriter(metadataStream))
            {
                var metadata = new
                {
                    package.PluginName,
                    package.Version,
                    package.MainAssembly,
                    package.EntryType,
                    package.Created,
                    AssemblyCount = package.Assemblies.Count,
                    ResourceCount = package.Resources.Count
                };
                var metadataJson = JsonSerializer.Serialize(metadata, JsonOptions);
                await metadataWriter.WriteAsync(metadataJson);
            }

            // Асинхронное сохранение сборок
            var assemblyTasks = package.Assemblies.Select(async assembly =>
            {
                var assemblyEntry = archive.CreateEntry($"assemblies/{assembly.Key}", compressionLevel);
                await using var assemblyStream = assemblyEntry.Open();
                await assemblyStream.WriteAsync(assembly.Value, 0, assembly.Value.Length);
            });

            await Task.WhenAll(assemblyTasks);
        }

        return memoryStream.ToArray();
    }

    // Вспомогательный метод для извлечения плагина из ZIP в директорию
    public static void ExtractZipToDirectory(byte[] zipData, string targetDirectory)
    {
        var package = DeserializeFromZip(zipData);
        package.SaveToDirectory(targetDirectory);
    }

    // Получение информации о плагине без полной распаковки
    public static PluginDeliveryInfo GetPluginInfo(byte[] zipData)
    {
        using var memoryStream = new MemoryStream(zipData);
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        var metadataEntry = archive.GetEntry("metadata.json");
        if (metadataEntry == null)
            throw new InvalidDataException("ZIP архив не содержит metadata.json");

        using var metadataStream = metadataEntry.Open();
        using var metadataReader = new StreamReader(metadataStream);
        var metadataJson = metadataReader.ReadToEnd();

        var metadata = JsonSerializer.Deserialize<PluginMetadata>(metadataJson, JsonOptions);

        return new PluginDeliveryInfo
        {
            Name = metadata.PluginName,
            Version = metadata.Version,
            MainAssembly = metadata.MainAssembly,
            EntryType = metadata.EntryType,
            Created = metadata.Created,
            AssemblyCount = metadata.AssemblyCount,
            ResourceCount = metadata.ResourceCount,
            ZipSize = zipData.Length
        };
    }
}
