namespace warkbench.src.basis.interfaces.Worlds;

/// <summary>
/// Manages the lifecycle and state of worlds within the active project.
/// </summary>
public interface IWorldService
{
    /// <summary> Creates a new world and registers it within the active project. </summary>
    IWorld CreateWorld(string name, int tileSize, int chunkResolution);
    
    /// <summary> Loads a specific world by its ID from the active project. </summary>
    void LoadWorld(Guid worldId);
    
    /// <summary> Persists all changes of the active world to disk. </summary>
    void SaveWorld();

    /// <summary> Unloads the active world and clears the current context. </summary>
    void CloseWorld();
    
    /// <summary> Deletes a world and its physical data from the project. </summary>
    void DeleteWorld(Guid worldId);
    
    /// <summary> The currently active world. Null if no world is loaded. </summary>
    IWorld? ActiveWorld { get; }
    
    /// <summary> Occurs when the active world has been changed or unloaded. </summary>
    event Action<IWorld?>? ActiveWorldChanged;
}