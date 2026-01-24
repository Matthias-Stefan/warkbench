using Avalonia.Controls;
using warkbench.src.editors.core.Worlds;

namespace warkbench.src.ui.core.Worlds;

/// <summary>Provides a dialog for collecting user input to create a new world.</summary>
public interface ICreateWorldDialog
{
    /// <summary>Shows the create project dialog and returns the user input or null if canceled.</summary>
    Task<CreateWorldInfo?> ShowAsync(Window owner);
}