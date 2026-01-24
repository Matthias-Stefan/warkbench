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
        ISelectionService<IWorld> worldSelectionService,
        IWorldService worldService)
    {
        _projectSession = projectSession;
        _projectSelectionService = projectSelectionService;
        _worldSelectionService = worldSelectionService;
        _worldService = worldService;
        
        _projectSelectionService.SelectionChanged += OnProjectSelectionChanged;
        _worldSelectionService.SelectionChanged += OnWorldSelectionChanged;
    }
    
    public void Dispose()
    {
        _projectSelectionService.SelectionChanged -= OnProjectSelectionChanged;
        _worldSelectionService.SelectionChanged -= OnWorldSelectionChanged;
    }
    
    public async Task OnCreateNewWorld()
    {
        var selectedProject = SelectedProject;
        if (selectedProject is null)
            return;

        var _ = await _worldService.CreateWorldAsync(selectedProject, "world", 32, 32);
        return;
    }

    private void OnProjectSelectionChanged(object? sender, SelectionChangedEventArgs<IProject> e)
    {
        SelectedProject = e.CurrentPrimary;
        
        Root = new TreeNodeViewModel(SelectedProject?.Name ?? string.Empty, "Project");

        Worlds = new TreeNodeViewModel(IProject.WorldsFolderName, "Worlds");
        Packages = new TreeNodeViewModel(IProject.PackagesFolderName, "Packages");
        Blueprints = new TreeNodeViewModel(IProject.BlueprintsFolderName,  "Blueprints");
        Properties = new TreeNodeViewModel(IProject.PropertiesFolderName,  "Properties");

        Root.AddChild(Worlds);
        Root.AddChild(Packages);
        Root.AddChild(Blueprints);
        Root.AddChild(Properties);

        RootLevel.Add(Root);
    }

    private void OnWorldSelectionChanged(object? sender, SelectionChangedEventArgs<IWorld> e)
    {
        
    }



    public ObservableCollection<ITreeNode> RootLevel { get; } = [];
    public ITreeNode? Root { get; private set; }
    public ITreeNode? Worlds { get; private set; }
    public ITreeNode? Packages { get; private set; }
    public ITreeNode? Blueprints { get; private set; }
    public ITreeNode? Properties { get; private set; }

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
    private readonly IWorldService _worldService;
}