using Avalonia.Controls;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.editors.core.ViewModel;

namespace warkbench.src.ui.core.Projects;

/// <summary>Desktop implementation of the create project dialog.</summary>
public class CreateProjectDialog(IPathService pathService) : ICreateProjectDialog
{
    /// <summary>Shows the create project dialog as a modal window.</summary>
    public async Task<CreateProjectResult?> ShowAsync(Window owner)
    {
        var viewModel = new CreateProjectViewModel(pathService);
        var window = new CreateProjectWindow
        {
            DataContext = viewModel
        };

        return await window.ShowDialog<CreateProjectResult?>(owner);
    }
}