using ExtensionEngine.Abstractions.Plugin;
using ExtensionEngine.Abstractions.Plugin.Models;
using ExtensionEngine.Core.Storage.Abstractions;
using System.Text.Json;

namespace ExtensionEngine.Core.Storage;

public class PluginContainerStorage : IPluginContainerStorage
{
    private readonly string _storageDirectory;
    private readonly string _metadataFileName = "plugins.json";

    private readonly JsonSerializerOptions _jsonOptions;

    public PluginContainerStorage()
    {
        _storageDirectory = "FilePluginContainerStorage";

        Directory.CreateDirectory(_storageDirectory);

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<IReadOnlyCollection<IPluginInfo>> GetPluginsAsync(CancellationToken cancellationToken)
    {
        var metadata = await GetPluginsListAsync(cancellationToken);

        return metadata?.Cast<IPluginInfo>().ToArray() ?? Array.Empty<IPluginInfo>();
    }

    public async Task<IPluginContainer> GetPluginContainerAsync(IPluginInfo plugin, CancellationToken cancellationToken)
    {
        if (plugin == null)
            throw new ArgumentNullException(nameof(plugin));

        var pluginFileName = GetPluginFileName(plugin);
        var pluginFilePath = Path.Combine(_storageDirectory, pluginFileName);

        if (!File.Exists(pluginFilePath))
        {
            throw new FileNotFoundException($"Plugin file not found: {pluginFilePath}");
        }

        var data = await File.ReadAllBytesAsync(pluginFilePath, cancellationToken);

        return new PluginContainer(plugin.Name, plugin.Version, data);
    }

    public async Task AddPluginsAsync(IReadOnlyCollection<IPluginContainer> pluginContainers, CancellationToken cancellationToken)
    {
        if (pluginContainers == null)
            throw new ArgumentNullException(nameof(pluginContainers));

        if (!pluginContainers.Any())
            return;

        // Получаем текущие метаданные
        var currentMetadata = await GetPluginsListAsync(cancellationToken);

        foreach (var pluginContainer in pluginContainers)
        {
            // Проверяем, существует ли уже плагин с таким именем и версией
            var existingPlugin = currentMetadata.FirstOrDefault(p =>
                p.Name == pluginContainer.Name && p.Version == pluginContainer.Version);

            if (existingPlugin != null)
            {
                throw new InvalidOperationException($"Plugin '{pluginContainer.Name}' version {pluginContainer.Version} already exists");
            }

            // Сохраняем данные плагина
            var pluginFileName = GetPluginFileName(pluginContainer);
            var pluginFilePath = Path.Combine(_storageDirectory, pluginFileName);
            await File.WriteAllBytesAsync(pluginFilePath, pluginContainer.Data, cancellationToken);

            // Добавляем метаданные
            currentMetadata.Add(new PluginInfo(pluginContainer.Name, pluginContainer.Version));
        }

        // Сохраняем обновленные метаданные
        await SaveMetadataAsync(currentMetadata, cancellationToken);
    }

    private async Task SaveMetadataAsync(List<PluginInfo> metadata, CancellationToken cancellationToken)
    {
        var metadataFilePath = Path.Combine(_storageDirectory, _metadataFileName);

        await using var fileStream = File.Create(metadataFilePath);
        await JsonSerializer.SerializeAsync(fileStream, metadata, _jsonOptions, cancellationToken);
    }

    private static string GetPluginFileName(IPluginInfo plugin)
    {
        var safeName = string.Join("_", plugin.Name.Split(Path.GetInvalidFileNameChars()));
        return $"{safeName}_v{plugin.Version}.plugin";
    }

    private async Task<List<PluginInfo>> GetPluginsListAsync(CancellationToken cancellationToken)
    {
        var metadataFilePath = Path.Combine(_storageDirectory, _metadataFileName);

        if (!File.Exists(metadataFilePath))
        {
            return new List<PluginInfo>();
        }

        await using var fileStream = File.OpenRead(metadataFilePath);
        var metadata = await JsonSerializer.DeserializeAsync<List<PluginInfo>>(fileStream, _jsonOptions, cancellationToken);

        return metadata ?? new List<PluginInfo>();
    }
}