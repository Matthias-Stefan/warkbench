using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.interfaces.Worlds;

/// <summary>Provides world creation, loading, persistence, and deletion services.</summary>
public interface IWorldService
{
    /// <summary>Creates a new world with the specified configuration.</summary>
    IWorld CreateWorld(IProject project, string name, int tileSize, int chunkResolution);
    
    /// <summary>Loads a world identified by the specified ID.</summary>
    IWorld LoadWorld(Guid worldId);
    
    /// <summary>Saves the specified world to persistent storage.</summary>
    void SaveWorld(IWorld world);
    
    /// <summary>Saves all dirty worlds from the specified set to persistent storage.</summary>
    void SaveAllDirty(IEnumerable<IWorld> worlds);
    
    /// <summary>Asynchronously saves all dirty worlds from the specified set to persistent storage.</summary>
    Task SaveAllDirtyAsync(IEnumerable<IWorld> worlds);
    
    /// <summary>Deletes the specified world and its associated data from storage.</summary>
    void DeleteWorld(IWorld world);
}