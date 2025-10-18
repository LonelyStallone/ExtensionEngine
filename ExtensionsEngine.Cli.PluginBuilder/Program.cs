

using ExtensionEngine.Plugin.Delivery;

var fileName = @"C:\Users\Victor\source\repos\ExtensionEngine\ExtensionEngine.Plugin.InventoryManagement.MacOs\bin\Release\net8.0\ExtensionEngine.Plugin.InventoryManagement.MacOs.dll";
var targetFolder = @"C:\Users\Victor\source\repos\ExtensionEngine\ExtensionEngine.Gateway\Resources";

var package = PluginPackage.CreateFromFile(fileName);
var data = PluginZipSerializer.SerializeToZip(package);

var pluginFileName = PluginNameBuilder.GetPluginFileName(package.PluginName, package.Version);
var targetFileName = Path.Combine(targetFolder, pluginFileName);

File.WriteAllBytes(targetFileName, data);