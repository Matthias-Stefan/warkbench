using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.interfaces.Worlds;

/// <summary>Provides world creation, loading, persistence, and deletion services.</summary>
public interface IWorldService
{
    /// <summary>Creates a new world instance with the specified configuration.</summary>
    IWorld CreateWorld(IProject project, string name, int tileSize, int chunkResolution);
    
    /// <summary>Asynchronously creates a new world instance with the specified configuration.</summary>
    Task<IWorld> CreateWorldAsync(IProject project, string name, int tileSize, int chunkResolution);
    
    /// <summary>Loads an existing world identified by the specified world ID.</summary>
    IWorld? LoadWorld(Guid worldId);

    /// <summary>Asynchronously loads an existing world identified by the specified world ID.</summary>
    Task<IWorld>? LoadWorldAsync(Guid worldId);
    
    /// <summary>Persists the specified world to storage.</summary>
    void SaveWorld(IWorld world);
    
    /// <summary>Asynchronously persists the specified world to storage.</summary>
    Task SaveWorldAsync(IWorld world);
    
    /// <summary>Saves all worlds marked as dirty from the specified collection.</summary>
    void SaveAllDirty(IEnumerable<IWorld> worlds);
    
    /// <summary>Asynchronously saves all worlds marked as dirty from the specified collection.</summary>
    Task SaveAllDirtyAsync(IEnumerable<IWorld> worlds);
    
    /// <summary>Removes the specified world and all associated data from storage.</summary>
    void DeleteWorld(IWorld world);
}