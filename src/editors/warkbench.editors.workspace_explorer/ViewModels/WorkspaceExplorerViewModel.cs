using Dock.Model.Mvvm.Controls;
using System.Collections.ObjectModel;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Selection;
using warkbench.src.basis.interfaces.Worlds;
using warkbench.src.editors.core.ViewModel;
using warkbench.src.editors.core.Worlds;

namespace warkbench.src.editors.workspace_explorer.ViewModels;

public class WorkspaceExplorerViewModel : Tool, IDisposable
{
    public WorkspaceExplorerViewModel(
        ILogger logger,
        IProjectSession projectSession, 
        ISelectionCoordinator selectionCoordinator,
        IWorldRepository worldRepository,
        IWorldService worldService
        )
    {
        _logger = logger;
        _projectSession = projectSession;
        _selectionCoordinator = selectionCoordinator;
        _worldRepository = worldRepository;
        _worldService = worldService;

        _selectionSubscription = selectionCoordinator.Subscribe(SelectionScope.Project);
        _selectionSubscription.Changed += OnSelectionChanged;
    }

    public void Dispose()
    {
        _selectionSubscription.Changed -= OnSelectionChanged;
        _selectionSubscription.Dispose();
    }
    
    public async Task OnCreateNewWorld(CreateWorldInfo createWorldInfo)
    {
        var selectedProject = _selectionCoordinator.CurrentProject;
        if (selectedProject is null)
        {
            _logger.Warn<WorkspaceExplorerViewModel>(
                "CreateWorld aborted: no project selected.");
            return;
        }

        var newWorld = await _worldService.CreateWorldAsync(
            selectedProject,
            createWorldInfo.WorldName,
            createWorldInfo.TileSize,
            createWorldInfo.ChunkResolution);

        if (Worlds is null)
        {
            _logger.Error<WorkspaceExplorerViewModel>(
                $"World '{newWorld.Name}' was created successfully, " +
                $"but WorkspaceExplorer has no Worlds tree. Project='{selectedProject.Name}'.");

            throw new InvalidOperationException(
                "WorkspaceExplorerViewModel.Worlds must not be null when creating a world.");
        }

        Worlds.AddChild(new TreeNodeViewModel(GetDisplayName(newWorld.LocalPath), newWorld));

        _logger.Info<WorkspaceExplorerViewModel>(
            $"World '{newWorld.Name}' added to workspace. " +
            $"Project='{selectedProject.Name}', TileSize={newWorld.TileSize}, " +
            $"ChunkResolution={newWorld.ChunkResolution}.");
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs<object> e)
    {
        if (e.CurrentPrimary is IProject project && !ReferenceEquals(_currentProject, project))
        {
            _currentProject = project;
            BuildProjectTree(project);
            return;
        }

        if (e.CurrentPrimary is not null || _currentProject is null) 
            return;
        
        _currentProject = null;
        ClearProjectTree();
    }
    
    private void BuildProjectTree(IProject project)
    {
        // Reset
        ClearProjectTree();
        
        // Root
        Root = new TreeNodeViewModel(GetDisplayName(project.LocalPath), project);

        // Folders
        Worlds = new TreeNodeViewModel(IProject.WorldsFolderName, "Worlds");
        Packages = new TreeNodeViewModel(IProject.PackagesFolderName, "Packages");
        Blueprints = new TreeNodeViewModel(IProject.BlueprintsFolderName,  "Blueprints");
        Properties = new TreeNodeViewModel(IProject.PropertiesFolderName,  "Properties");

        Root.AddChild(Worlds);
        Root.AddChild(Packages);
        Root.AddChild(Blueprints);
        Root.AddChild(Properties);

        // World nodes (manifest paths)
        foreach (var worldPath in project.Worlds)
        {
            var displayName = GetDisplayName(worldPath);
            Worlds.AddChild(new TreeNodeViewModel(displayName, worldPath));
        }

        RootLevel.Add(Root);
    }

    private void ClearProjectTree()
    {
        RootLevel.Clear();
    }
    
    private static string GetDisplayName(LocalPath path)
    {
        return path.IsEmpty ? string.Empty : Path.GetFileName(path.Value);
    }

    public ObservableCollection<ITreeNode> RootLevel { get; } = [];
    public ITreeNode? Root { get; private set; }
    public ITreeNode? Worlds { get; private set; }
    public ITreeNode? Packages { get; private set; }
    public ITreeNode? Blueprints { get; private set; }
    public ITreeNode? Properties { get; private set; }

    public TreeNodeViewModel? Selected
    {
        get => _selected;
        set
        {
            SetProperty(ref _selected, value);
            switch (_selected?.Data)
            {
                case IProject project:
                    _selectionCoordinator.SelectProject(project);
                    break;
                case IWorld world:
                    _selectionCoordinator.SelectWorld(world);
                    break;
            }
        }
    }

    private IProject? _currentProject;
    
    private TreeNodeViewModel? _selected;
    private readonly ISelectionSubscription _selectionSubscription;
    
    private readonly ILogger _logger;
    private readonly IProjectSession _projectSession;
    private readonly ISelectionCoordinator _selectionCoordinator;
    private readonly IWorldRepository _worldRepository;
    private readonly IWorldService _worldService;
}