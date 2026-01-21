using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;
using warkbench.src.editors.core.ViewModel;

namespace warkbench.src.editors.workspace_explorer.ViewModels;

public class WorkspaceExplorerViewModel : Tool, IDisposable
{
    public WorkspaceExplorerViewModel(
        IProjectSession projectSession, 
        ISelectionService<IProject> projectSelectionService,
        ISelectionService<IWorld> worldSelectionService)
    {
        _projectSession = projectSession;
        _projectSelectionService = projectSelectionService;
        _worldSelectionService = worldSelectionService;

        Root = new TreeNodeViewModel("Project", "Worlds");

        Worlds = new TreeNodeViewModel("Worlds", "Worlds");
        Packages = new TreeNodeViewModel("Packages", "Packages");
        Blueprints = new TreeNodeViewModel("Blueprints",  "Blueprints");
        Properties = new TreeNodeViewModel("Properties",  "Properties");

        Root.AddChild(Worlds);
        Root.AddChild(Packages);
        Root.AddChild(Blueprints);
        Root.AddChild(Properties);

        RootLevel = [ Root ];

        _projectSelectionService.SelectionChanged += OnProjectSelectionChanged;
        _worldSelectionService.SelectionChanged += OnWorldSelectionChanged;
    }
    
    public void Dispose()
    {
        _projectSelectionService.SelectionChanged -= OnProjectSelectionChanged;
        _worldSelectionService.SelectionChanged -= OnWorldSelectionChanged;
    }

    private void OnProjectSelectionChanged(object? sender, SelectionChangedEventArgs<IProject> e)
    {
        SelectedProject = e.CurrentPrimary;
        
        
    }

    private void OnWorldSelectionChanged(object? sender, SelectionChangedEventArgs<IWorld> e)
    {
        
    }
    
    public ObservableCollection<ITreeNode> RootLevel { get; init; }
    public ITreeNode Root { get; }
    public ITreeNode Worlds { get; }
    public ITreeNode Packages { get; }
    public ITreeNode Blueprints { get; }
    public ITreeNode Properties { get; }

    public object? Selected { get; private set; }

    public IProject? SelectedProject
    {
        get => _projectSession.Current;
        set
        {
            _projectSession.SwitchTo(value);
            Selected = value;
        }
        
    }

    private readonly IProjectSession _projectSession;
    
    private readonly ISelectionService<IProject> _projectSelectionService;
    private readonly ISelectionService<IWorld> _worldSelectionService;
}