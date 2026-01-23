using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.interfaces.Io;

public interface IProjectIoService : IIoService
{
    /// <summary> 
    /// Updates an existing project instance with data from the disk. 
    /// Uses Newtonsoft.Json.Populate to maintain object references.
    /// </summary>
    void PopulateProject(AbsolutePath path, IProject target);
    
    /// <summary> The standard file extension for warkbench projects. </summary>
    const string Extension = ".wbproj";
}