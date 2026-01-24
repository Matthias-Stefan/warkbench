using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.interfaces.Io;

public interface IWorldIoService : IIoService<IWorld>
{
    /// <summary> Updates an existing world instance with data from disk (PopulateObject) to preserve references. </summary>
    void PopulateWorld(AbsolutePath path, IWorld target);

    /// <summary> Asynchronously updates an existing world instance with data from disk (PopulateObject) to preserve references. </summary>
    Task PopulateWorldAsync(AbsolutePath path, IWorld target);
    
    /// <summary> Scans a directory for all available workbench world files. </summary>
    IEnumerable<string> DiscoverWorlds(AbsolutePath searchPath, bool recursive = true);
    
    /// <summary> The standard file extension for warkbench world. </summary>
    const string Extension = ".wbworld";
}