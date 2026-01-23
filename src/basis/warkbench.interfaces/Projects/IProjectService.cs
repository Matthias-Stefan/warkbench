using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.interfaces.Projects;

/// <summary>Provides project creation, loading, persistence, and deletion services.</summary>
public interface IProjectService
{
    /// <summary>Creates a new project and initializes its directory structure.</summary>
    IProject CreateProject(string name);
    
    /// <summary>Loads and deserializes a project from the specified path.</summary>
    IProject LoadProject(LocalPath localPath);

    /// <summary>Asynchronously loads and deserializes a project from the specified path.</summary> 
    Task<IProject> LoadProjectAsync(LocalPath localPath);
    
    /// <summary>Saves the specified project to persistent storage.</summary>
    void SaveProject(IProject project);

    /// <summary>Deletes the specified project and its associated data from storage.</summary>
    void DeleteProject(IProject project);
}