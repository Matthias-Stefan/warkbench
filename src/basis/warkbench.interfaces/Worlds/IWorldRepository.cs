using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.interfaces.Worlds;

/// <summary> Maintains an in-memory registry of loaded world instances, keyed by project-relative paths. </summary>
public interface IWorldRepository
{
    /// <summary>Attempts to retrieve a loaded world by its project-relative path.</summary>
    bool TryGet(LocalPath path, out IWorld? world);

    /// <summary>Registers a loaded world instance under the specified project-relative path.</summary>
    void Add(LocalPath path, IWorld world);

    /// <summary>Checks whether the specified local path exists as a key.</summary>
    bool ContainsKey(LocalPath path);
    
    /// <summary>Removes the loaded world instance associated with the specified path.</summary>
    bool Remove(LocalPath path);

    /// <summary>Removes all loaded world instances from the repository.</summary>
    void Clear();

    /// <summary>Gets a read-only collection of all currently loaded world instances.</summary>
    IReadOnlyCollection<IWorld> Worlds { get; }
}