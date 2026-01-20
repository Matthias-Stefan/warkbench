using System.Collections.ObjectModel;
using Dock.Model.Mvvm.Controls;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;
using warkbench.src.editors.core.ViewModel;

namespace warkbench.src.editors.project_explorer.ViewModel;

public class ProjectExplorerViewModel : Tool, IDisposable
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

        Worlds = new TreeNodeViewModel("Worlds", null);
        Packages = new TreeNodeViewModel("Packages", null);
        Blueprints = new TreeNodeViewModel("Blueprints", null);
        Properties = new TreeNodeViewModel("Properties", null);
        
        Assets = [ Worlds, Packages, Blueprints, Properties ];

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
        SelectedWorld = e.CurrentPrimary;
    }
    
    public ITreeNode Worlds { get; }
    public ITreeNode Packages { get; }
    public ITreeNode Blueprints { get; }
    public ITreeNode Properties { get; }
    
    public ObservableCollection<ITreeNode> Assets { get; init; }

    public IProject? SelectedProject
    {
        get => _selectedProject;
        set
        {
            if (SelectedProject is not null)
                _projectSelectionService.Deselect(SelectedProject);
            
            if (value is null)
            {
                foreach (var worldNode in Worlds.Children)
                {
                    if (worldNode.Data is not IWorld world)
                        continue;
                    
                    _worldService.SaveWorld();
                }
                    
                _projectService.SaveProject();
                _projectService.CloseProject();
                return;
            }
            
            
            
            // new selection
            _selectedProject = value;
            _projectService.LoadProject(value.LocalPath);
            _projectSelectionService.Select(value);
        }
    }

    public IWorld? SelectedWorld
    {
        get => _selectedWorld;
        set
        {
            
        }
    }
    
    private readonly IProjectService _projectService;
    private readonly IWorldService _worldService;
    
    private readonly ISelectionService<IProject> _projectSelectionService;
    private readonly ISelectionService<IWorld> _worldSelectionService;

    private IProject? _selectedProject;
    private IWorld? _selectedWorld;
}