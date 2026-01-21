using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Projects;

public class ProjectSession : IProjectSession, IDisposable
{
    public ProjectSession(
        ISelectionService<IProject> projectSelection,
        IProjectService projectService,
        IWorldService worldService)
    {
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
    {
        if (_isSwitching)
            return;

        var current = _projectSelection.Primary;
        if (ReferenceEquals(current, project))
            return;

        try
        {
            _isSwitching = true;

            // 1) Save current session (if any)
            if (current is not null)
            {
                SaveAllDirty();
                _projectService.SaveProject(current);
                _projectSelection.Deselect(current);
            }

            // 2) Close
            if (project is null)
            {
                CurrentChanged?.Invoke(null);
                return;
            }

            // 3) Load and publish the loaded instance
            var loaded = _projectService.LoadProject(project.LocalPath);
            _projectSelection.Select(loaded);

            CurrentChanged?.Invoke(loaded);
        }
        finally
        {
            _isSwitching = false;
        }
    }

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
                await SaveAllDirtyAsync().ConfigureAwait(false);
                _projectService.SaveProject(current);
                _projectSelection.Deselect(current);
            }

            if (project is null)
            {
                CurrentChanged?.Invoke(null);
                return;
            }

            var loaded = await _projectService.LoadProjectAsync(project.LocalPath)
                .ConfigureAwait(false);

            _projectSelection.Select(loaded);
            CurrentChanged?.Invoke(loaded);
        }
        finally
        {
            _isSwitching = false;
        }
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
    
    private readonly ISelectionService<IProject> _projectSelection;
    private readonly IProjectService _projectService;
    private readonly IWorldService _worldService;

    private bool _isSwitching;
}