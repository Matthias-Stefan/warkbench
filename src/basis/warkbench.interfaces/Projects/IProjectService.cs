namespace warkbench.src.basis.interfaces.Projects;

/// <summary>
/// Service for managing the lifecycle, persistence, and state of workbench projects.
/// </summary>
public interface IProjectService
{
    /// <summary> Initializes a new project and creates the physical directory structure. </summary>
    IProject CreateProject(string name);
    
    /// <summary> Deserializes a project file and sets it as the active session instance. </summary>
    void LoadProject(string path);

    /// <summary> Deserializes a project file asynchronously and sets it as the active session instance. </summary> 
    Task LoadProjectAsync(string path);
    
    /// <summary> Persists the state of the active project to the underlying storage. </summary>
    void SaveProject();

    /// <summary> Discards the active project instance and clears session-related memory. </summary>
    void CloseProject();

    /// <summary> Physically removes project-related data from the file system. </summary>
    void DeleteProject(string path);
    
    /// <summary> The current project instance associated with the active editor session. </summary>
    IProject? ActiveProject { get; }
    
    /// <summary> Occurs when the active project instance is loaded, closed, or swapped. </summary>
    event Action<IProject?> ActiveProjectChanged;
}