using System.ComponentModel;
using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.interfaces.Projects;

/// <summary>Coordinates project switching and workspace state transitions.</summary>
public interface IProjectSession
{
    /// <summary>Opens and activates the given project using the specified cascading load mode.</summary>
    Task<IProject?> OpenAsync(AbsolutePath projectPath, ProjectLoadMode mode);

    /// <summary>Opens and activates the given project using the specified cascading load mode.</summary>
    Task<IProject?> OpenAsync(LocalPath projectPath, ProjectLoadMode mode);

    /// <summary>Closes the currently active project (if any) and activates the specified project without reloading it.</summary>
    Task ActivateAsync(IProject project);
    
    /// <summary>Saves the given project and all associated data using a cascading workflow.</summary>
    Task SaveAsync(IProject? project);

    /// <summary>Closes the given project and persists its state.</summary>
    Task CloseAsync(IProject? project);
}