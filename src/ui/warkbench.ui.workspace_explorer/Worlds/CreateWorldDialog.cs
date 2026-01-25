using Avalonia.Controls;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.editors.core.Worlds;

namespace warkbench.src.ui.workspace_explorer.Worlds;

/// <summary>Desktop implementation of the create world dialog.</summary>
public class CreateWorldDialog : ICreateWorldDialog
{
    /// <summary>Shows the create world dialog as a modal window.</summary>
    public async Task<CreateWorldInfo?> ShowAsync(Window owner)
    {
        var viewModel = new CreateWorldViewModel();
        var window = new CreateWorldWindow
        {
            DataContext = viewModel
        };
        
        return await window.ShowDialog<CreateWorldInfo?>(owner);
    }
}