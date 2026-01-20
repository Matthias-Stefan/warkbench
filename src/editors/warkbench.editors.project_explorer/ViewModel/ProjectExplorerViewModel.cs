using System.Collections.ObjectModel;
using Dock.Model.Mvvm.Controls;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;
using warkbench.src.editors.core.ViewModel;

namespace warkbench.src.editors.project_explorer.ViewModel;

public class ProjectExplorerViewModel : Tool
{
    public ProjectExplorerViewModel(
        IProjectService projectService, 
        IWorldService worldService,
        ISelectionService<IProject> projectSelectionService,
        ISelectionService<IWorld> worldSelectionService)
    {
        _projectService = projectService;
        _worldService = worldService;
        _projectSelectionService = projectSelectionService;
        _worldSelectionService = worldSelectionService;

        Projects = new TreeNodeViewModel("Projects", null);
        Worlds = new TreeNodeViewModel("Worlds", null);
        Packages = new TreeNodeViewModel("Packages", null);
        Blueprints = new TreeNodeViewModel("Blueprints", null);
        Properties = new TreeNodeViewModel("Properties", null);
        
        Assets = [ Projects, Worlds, Packages, Blueprints, Properties ];
    }




    public ITreeNode Projects { get; }
    public ITreeNode Worlds { get; }
    public ITreeNode Packages { get; }
    public ITreeNode Blueprints { get; }
    public ITreeNode Properties { get; }

    public ObservableCollection<ITreeNode> Assets { get; init; }

    private readonly IProjectService _projectService;
    private readonly IWorldService _worldService;
    
    private readonly ISelectionService<IProject> _projectSelectionService;
    private readonly ISelectionService<IWorld> _worldSelectionService;
}