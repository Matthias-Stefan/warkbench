using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.interfaces.Projects;

/// <summary>Provides project creation, loading, persistence, and deletion services.</summary>
public interface IProjectService
{
    /// <summary>Asynchronously creates a new project and initializes its directory structure.</summary>
    Task<IProject> CreateProjectAsync(string name);

    /// <summary>Asynchronously loads and deserializes a project from an absolute file system path.</summary> 
    Task<IProject> LoadProjectAsync(AbsolutePath absolutePath);

    /// <summary>Asynchronously saves the specified project to persistent storage.</summary>
    Task SaveProjectAsync(IProject project);

    /// <summary>Deletes the specified project and its associated data from storage.</summary>
    Task DeleteProjectAsync(IProject project);
}