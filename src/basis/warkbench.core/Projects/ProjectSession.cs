using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Projects;

public class ProjectSession : IProjectSession, IDisposable
{
    public ProjectSession(
        IAppStateService appStateService,
        ISelectionService<IProject> projectSelection,
        IProjectService projectService,
        IWorldService worldService)
    {
        _appStateService = appStateService;
        _projectSelection = projectSelection;
        _projectService = projectService;
        _worldService = worldService;

        _projectSelection.SelectionChanged += OnSelectionChanged;
    }

    public void Dispose()
    {
        _projectSelection.SelectionChanged -= OnSelectionChanged;
    }
    
    public void SwitchTo(IProject? project)
        => SwitchToAsync(project).GetAwaiter().GetResult();

    public async Task SwitchToAsync(IProject? project)
    {
        if (_isSwitching)
            return;

        var current = _projectSelection.Primary;
        if (ReferenceEquals(current, project))
            return;

        try
        {
            _isSwitching = true;

            if (current is not null)
            {
                await SaveAllDirtyAsync();
                await _projectService.SaveProjectAsync(current);
                _projectSelection.Deselect(current);
            }

            if (project is null)
            {
                CurrentChanged?.Invoke(null);
                return;
            }

            _projectSelection.Select(project);
            PersistLastProjectPath(project.LocalPath);
            CurrentChanged?.Invoke(project);
        }
        finally
        {
            _isSwitching = false;
        }
    }
    
    private void PersistLastProjectPath(LocalPath path)
    {
        _appStateService.State.LastProjectPath = path;
        _appStateService.Save();
    }
    
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs<IProject> e)
    {
        if (ReferenceEquals(e.CurrentPrimary, e.PreviousPrimary))
            return;

        CurrentChanged?.Invoke(e.CurrentPrimary);
    }
    
    private void SaveAllDirty()
    {
        if (_projectSelection.Primary is null)
            return;
        
        _worldService.SaveAllDirty(_projectSelection.Primary.Worlds);
        // TODO: scenes, assets, etc.
    }
    
    private Task SaveAllDirtyAsync()
    {
        if (_projectSelection.Primary is null)
            return Task.CompletedTask;
        
        return _worldService.SaveAllDirtyAsync(_projectSelection.Primary.Worlds);
        // TODO: scenes, assets, etc.
    }
    
    public IProject? Current => _projectSelection.Primary;

    public event Action<IProject?>? CurrentChanged;
    
    private readonly IAppStateService _appStateService;
    private readonly ISelectionService<IProject> _projectSelection;
    private readonly IProjectService _projectService;
    private readonly IWorldService _worldService;

    private bool _isSwitching;
}