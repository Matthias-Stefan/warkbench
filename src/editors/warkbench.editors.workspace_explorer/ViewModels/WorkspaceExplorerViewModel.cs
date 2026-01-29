using Avalonia.Threading;
using Dock.Model.Mvvm.Controls;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using warkbench.src.basis.interfaces.Logger;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Selection;
using warkbench.src.basis.interfaces.Worlds;
using warkbench.src.editors.core.Models;
using warkbench.src.editors.core.ViewModel;
using warkbench.src.editors.core.Worlds;

namespace warkbench.src.editors.workspace_explorer.ViewModels;

public partial class WorkspaceExplorerViewModel : Tool, IDisposable
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
        var selectedProject = _selectionCoordinator.LastSelectedProject;
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

        var payload = new TreeNodePayload(GetDisplayName(newWorld.LocalPath), newWorld, LoadState.Loaded);
        Worlds.AddChild(new TreeNodeViewModel(payload));
        
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
            _ = BuildProjectTree(project);
            return;
        }

        if (e.CurrentPrimary is not null || _currentProject is null) 
            return;
        
        _currentProject = null;
        ClearProjectTree();
    }
    
    private async Task BuildProjectTree(IProject project)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            ClearProjectTree();

            Root = new TreeNodeViewModel(new TreeNodePayload(GetDisplayName(project.LocalPath), project, null));

            Worlds     = new TreeNodeViewModel(new TreeNodePayload(IProject.WorldsFolderName, "Worlds", null));
            Packages   = new TreeNodeViewModel(new TreeNodePayload(IProject.PackagesFolderName, "Packages", null));
            Blueprints = new TreeNodeViewModel(new TreeNodePayload(IProject.BlueprintsFolderName, "Blueprints", null));
            Properties = new TreeNodeViewModel(new TreeNodePayload(IProject.PropertiesFolderName, "Properties", null));

            Root.AddChild(Worlds);
            Root.AddChild(Packages);
            Root.AddChild(Blueprints);
            Root.AddChild(Properties);

            foreach (var worldPath in project.Worlds)
            {
                var displayName = GetDisplayName(worldPath);
                Worlds.AddChild(_worldRepository.TryGet(worldPath, out var world)
                    ? new TreeNodeViewModel(new TreeNodePayload(displayName, world, LoadState.Loaded))
                    : new TreeNodeViewModel(new TreeNodePayload(displayName, worldPath, LoadState.NotLoaded)));
            }

            RootLevel.Clear();
            RootLevel.Add(Root);
        });
    }
    
    private void ClearProjectTree()
    {
        RootLevel.Clear();
    }
    
    private static string GetDisplayName(LocalPath path)
        => path.IsEmpty ? string.Empty : Path.GetFileName(path.Value);

    public ObservableCollection<TreeNodeViewModel> RootLevel { get; } = [];
    private TreeNodeViewModel? Root { get; set; }
    private TreeNodeViewModel? Worlds { get; set; }
    private TreeNodeViewModel? Packages { get; set; }
    private TreeNodeViewModel? Blueprints { get; set; }
    private TreeNodeViewModel? Properties { get; set; }

    public TreeNodeViewModel? Selected
    {
        get => _selected;
        set
        {
            SetProperty(ref _selected, value);
            
            if (_selected is null)
                return;

            _ = HandleSelectionAsync(_selected);
        }
    }
    
    private async Task HandleSelectionAsync(TreeNodeViewModel selected)
    {
        if (selected.Data is null)
            throw new InvalidDataException("Selected data is null.");

        // World lazy-load
        if (selected.Data is LocalPath localPath)
        {
            selected.LoadState = LoadState.Loading;
            var world = await _worldService.LoadWorldAsync(
                _currentProject!, localPath);

            selected.Data = world;
            selected.LoadState = LoadState.Loaded;
        }

        switch (selected.Data)
        {
            case IProject project:
                _selectionCoordinator.SelectProject(project);
                break;

            case IWorld world:
                _selectionCoordinator.SelectWorld(world);
                break;
        }
    }

    [RelayCommand]
    private async Task OnSave(IProject project)
    {
        await _projectSession.SaveAsync(project);
    }
    
    [RelayCommand]
    private Task OnBeginRename(IProject project)
    {
        _selected?.BeginRename();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task OnCommitRename(string newName)
    {
        if (_selected?.Data is not IProject project || string.IsNullOrEmpty(newName))
            return;
        
        await _projectSession.RenameAsync(project, newName);
    }

    [RelayCommand]
    private Task OnCancelRename()
    {
        _selected?.CancelRename();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnClose(IProject project)
    {
        return Task.CompletedTask;
    }
    
    [RelayCommand]
    private Task OnDelete(IProject project)
    {
        return Task.CompletedTask;
    }
    
    [RelayCommand]
    private Task OnCopyPath(IProject project)
    {
        return Task.CompletedTask;
    }
    
    [RelayCommand]
    private Task OnOpenContainingFolder(IProject project)
    {
        return Task.CompletedTask;
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