using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.interfaces.Common;

public interface IProjectIoService : IIoService
{
    /// <summary> 
    /// Updates an existing project instance with data from the disk. 
    /// Uses Newtonsoft.Json.Populate to maintain object references.
    /// </summary>
    void PopulateProject(string path, IProject target);
    
    /// <summary> Scans a directory for all available workbench project files. </summary>
    IEnumerable<string> DiscoverProjects(string searchPath, bool recursive = true);
    
    /// <summary> The standard file extension for warkbench projects. </summary>
    const string Extension = ".wbproj";
}