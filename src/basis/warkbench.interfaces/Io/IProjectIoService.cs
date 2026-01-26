using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.interfaces.Io;

public interface IProjectIoService : IIoService<IProject>
{
    /// <summary> Asynchronously updates an existing project instance with data from disk (PopulateObject) to preserve references. </summary>
    Task PopulateProjectAsync(AbsolutePath path, IProject target);
    
    /// <summary>The standard file extension for warkbench projects.</summary>
    const string Extension = ".wbproj";
}