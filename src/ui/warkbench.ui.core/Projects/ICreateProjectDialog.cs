using Avalonia.Controls;
using warkbench.src.editors.core.Projects;
using warkbench.src.editors.core.Worlds;

namespace warkbench.src.ui.core.Projects;

/// <summary>Provides a dialog for collecting user input to create a new project.</summary>
public interface ICreateProjectDialog
{
    /// <summary>Shows the create project dialog and returns the user input or null if canceled.</summary>
    Task<CreateProjectInfo?> ShowAsync(Window owner);
}