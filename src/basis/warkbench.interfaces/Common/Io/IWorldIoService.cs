using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.interfaces.Common;

public interface IWorldIoService : IIoService
{
    /// <summary> 
    /// Updates an existing world instance with data from the disk. 
    /// Uses Newtonsoft.Json.Populate to maintain object references.
    /// </summary>
    void PopulateWorld(string path, IWorld target);
    
    /// <summary> Scans a directory for all available workbench world files. </summary>
    IEnumerable<string> DiscoverWorlds(string searchPath, bool recursive = true);
    
    /// <summary> The standard file extension for warkbench world. </summary>
    const string Extension = ".wbworld";
}