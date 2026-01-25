using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Selection;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Projects;

public class ProjectSession : IProjectSession, IDisposable
{
    public ProjectSession(
        IAppStateService appStateService,
        IProjectService projectService,
        ISelectionCoordinator selectionCoordinator,
        IWorldService worldService
        )
    {
        _appStateService = appStateService;
        _projectService = projectService;
        _selectionCoordinator = selectionCoordinator;
        _worldService = worldService;

        _selectionSubscription = selectionCoordinator.Subscribe(SelectionScope.Project);
        _selectionSubscription.Changed += OnSelectionChanged;
    }

    public void Dispose()
    {
        _selectionSubscription.Changed -= OnSelectionChanged;
        _selectionSubscription.Dispose();
    }
    
    public void SwitchTo(IProject? project)
        => SwitchToAsync(project).GetAwaiter().GetResult();

    public async Task SwitchToAsync(IProject? project)
    {
        var current = _selectionCoordinator.CurrentProject;
        if (ReferenceEquals(current, project))
            return;

        if (current is not null)
        {
            await _worldService.SaveAllDirtyAsync(current.Worlds);
            await _projectService.SaveProjectAsync(current);
        }

        _selectionCoordinator.SelectProject(project);
        PersistLastProjectPath(project?.LocalPath ?? new LocalPath());
    }
    
    private void PersistLastProjectPath(LocalPath path)
    {
        _appStateService.State.LastProjectPath = path;
        _appStateService.Save();
    }
    
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs<object> e)
        => SwitchTo(_selectionCoordinator.CurrentProject);
    
    private readonly ISelectionSubscription _selectionSubscription;
    
    private readonly IAppStateService _appStateService;
    private readonly IProjectService _projectService;
    private readonly ISelectionCoordinator _selectionCoordinator;
    private readonly IWorldService _worldService;
}