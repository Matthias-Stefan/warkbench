using Avalonia.Controls;
using warkbench.src.editors.core.ViewModel;

namespace warkbench.src.ui.core.Projects;

/// <summary>Provides a dialog for collecting user input to create a new project.</summary>
public interface ICreateProjectDialog
{
    /// <summary>Shows the create project dialog and returns the user input or null if cancelled.</summary>
    Task<CreateProjectResult?> ShowAsync(Window owner);
}