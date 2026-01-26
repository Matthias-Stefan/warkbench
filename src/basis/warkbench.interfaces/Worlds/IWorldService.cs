using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.interfaces.Worlds;

/// <summary>
/// Provides services for creating, loading, caching, and persisting world instances.
/// </summary>
public interface IWorldService
{
    /// <summary>Asynchronously creates a new world and registers it with the specified project.</summary>
    Task<IWorld> CreateWorldAsync(IProject project, string name, int tileSize, int chunkResolution);
    
    /// <summary>
    /// Asynchronously loads the specified world for the given project.
    /// If the world is already loaded, the existing instance is returned.
    /// </summary>
    Task<IWorld> LoadWorldAsync(IProject project, LocalPath worldPath);

    /// <summary>Asynchronously persists the specified world to storage.</summary>
    Task SaveWorldAsync(IWorld world);

    /// <summary>Asynchronously saves all loaded worlds that are marked as dirty.</summary>
    Task SaveAllDirtyAsync(IEnumerable<IWorld> worlds);

    /// <summary>Asynchronously deletes the specified world and all associated data.</summary>
    Task DeleteWorldAsync(IWorld world);
}