using Avalonia.Controls;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.editors.core.Worlds;
using warkbench.src.ui.core.Projects;

namespace warkbench.src.ui.core.Worlds;

/// <summary>Desktop implementation of the create world dialog.</summary>
public class CreateWorldDialog(IPathService pathService, IProjectSession projectSession) : ICreateWorldDialog
{
    /// <summary>Shows the create world dialog as a modal window.</summary>
    public async Task<CreateWorldInfo?> ShowAsync(Window owner)
    {
        var viewModel = new CreateWorldViewModel(pathService, projectSession);

        return null;
    }
}