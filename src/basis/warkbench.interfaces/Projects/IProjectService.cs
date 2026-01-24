using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.interfaces.Projects;

/// <summary>Provides project creation, loading, persistence, and deletion services.</summary>
public interface IProjectService
{
    /// <summary>Creates a new project and initializes its directory structure.</summary>
    IProject CreateProject(string name);

    /// <summary>Asynchronously creates a new project and initializes its directory structure.</summary>
    Task<IProject> CreateProjectAsync(string name);

    /// <summary>Loads and deserializes a project from an absolute file system path.</summary>
    IProject? LoadProject(AbsolutePath absolutePath);

    /// <summary>Asynchronously loads and deserializes a project from an absolute file system path.</summary> 
    Task<IProject?> LoadProjectAsync(AbsolutePath absolutePath);

    /// <summary>Loads and deserializes a project from a project-relative local path.</summary>
    IProject? LoadProject(LocalPath localPath);

    /// <summary>Asynchronously loads and deserializes a project from a project-relative local path.</summary> 
    Task<IProject?> LoadProjectAsync(LocalPath localPath);

    /// <summary>Saves the specified project to persistent storage.</summary>
    void SaveProject(IProject project);

    /// <summary>Asynchronously saves the specified project to persistent storage.</summary>
    Task SaveProjectAsync(IProject project);

    /// <summary>Deletes the specified project and its associated data from storage.</summary>
    void DeleteProject(IProject project);
}