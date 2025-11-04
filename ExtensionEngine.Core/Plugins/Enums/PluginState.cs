namespace ExtensionEngine.Core.Plugins.Enums;

/// <summary>
/// Main states of a plugin in a host process
/// </summary>
public enum PluginState
{
    /// <summary>
    /// Plugin is not initialized
    /// </summary>
    NotInitialized = 0,

    /// <summary>
    /// Plugin is initialized and ready to work
    /// </summary>
    Ready = 1,

    /// <summary>
    /// Plugin is running and working
    /// </summary>
    Running = 2,

    /// <summary>
    /// Plugin is stopped
    /// </summary>
    Stopped = 3,

    /// <summary>
    /// Plugin is in error state
    /// </summary>
    Error = 4,

    /// <summary>
    /// Plugin has finished work and released resources
    /// </summary>
    Disposed = 5
}