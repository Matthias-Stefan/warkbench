namespace warkbench.src.basis.interfaces.Projects;

/// <summary>Provides a service for selecting an existing project file.</summary>
public interface IProjectPicker
{
    /// <summary>Opens a file picker and returns the selected project file path or <c>null</c> if cancelled.</summary>
    Task<string?> Open();
}