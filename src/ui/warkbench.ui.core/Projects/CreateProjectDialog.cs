using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.editors.core.Projects;
using warkbench.src.ui.core.Window;

namespace warkbench.src.ui.core.Projects;

/// <summary>Desktop implementation of the create project dialog.</summary>
public class CreateProjectDialog(
    MainWindowProvider mainWindowProvider,
    IPathService pathService) : ICreateProjectDialog
{
    public async Task<CreateProjectInfo?> ShowAsync()
    {
        var viewModel = new CreateProjectViewModel(pathService);
        var window = new CreateProjectWindow
        {
            DataContext = viewModel
        };

        return await window.ShowDialog<CreateProjectInfo?>(mainWindowProvider.Get!);
    }
}