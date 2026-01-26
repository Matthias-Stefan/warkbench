using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Selection;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Projects;

public class ProjectSession(
    IAppStateService appStateService,
    ILogger logger,
    IPathService pathService,
    IProjectService projectService,
    ISelectionCoordinator selectionCoordinator,
    IWorldService worldService)
    : IProjectSession
{
    public async Task<IProject?> OpenAsync(AbsolutePath projectPath, ProjectLoadMode mode)
    {
        if (projectPath.IsEmpty)
            throw new ArgumentException("Project path must not be empty.", nameof(projectPath));

        if (_isBusy)
            return null;

        try
        {
            _isBusy = true;

            var current = selectionCoordinator.CurrentProject;
            if (current is not null)
                await CloseAsync(current).ConfigureAwait(false);

            var project = await LoadAsync(projectPath, mode).ConfigureAwait(false);
            if (project is null)
                return null;

            await SwitchToAsync(project);
            return project;
        }
        finally
        {
            _isBusy = false;
        }
    }

    public async Task<IProject?> OpenAsync(LocalPath projectPath, ProjectLoadMode mode)
    {
        if (projectPath.IsEmpty)
        {
            const string errorMsg = "Project path cannot be null or empty.";
            logger.Error<ProjectService>(errorMsg);
            throw new ArgumentException(errorMsg, nameof(projectPath));
        }
        
        var absolutePath = new AbsolutePath(
            UnixPath.Combine(pathService.ProjectsPath.Value, projectPath.Value)
        );

        return await OpenAsync(absolutePath, mode);
    }

    public async Task ActivateAsync(IProject project)
    {
        if (_isBusy)
            return;

        try
        {
            _isBusy = true;

            var current = selectionCoordinator.CurrentProject;
            if (ReferenceEquals(current, project))
                return;

            if (current is not null)
                await CloseAsync(current).ConfigureAwait(false);

            await SwitchToAsync(project);
        }
        finally
        {
            _isBusy = false;
        }
    }

    public Task SaveAsync(IProject? project)
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync(IProject? project)
    {
        throw new NotImplementedException();
    }

    private async Task<IProject?> LoadAsync(AbsolutePath projectPath, ProjectLoadMode mode)
    {
        // 1) Load manifest
        var project = await projectService.LoadProjectAsync(projectPath).ConfigureAwait(false);
        if (mode == ProjectLoadMode.ManifestOnly)
            return project;

        // 2) Load startup/active world (if any)
        var activeWorldPath = project.ActiveWorldPath;
        if (activeWorldPath is not null && !activeWorldPath.Value.IsEmpty)
        {
            await worldService.LoadWorldAsync(project, (LocalPath)activeWorldPath).ConfigureAwait(false);
        }
        if (mode == ProjectLoadMode.StartupWorld)
            return project;
        
        // 3) Load remaining worlds
        foreach (var worldPath in project.Worlds)
        {
            if (activeWorldPath is not null && worldPath.Equals(activeWorldPath.Value))
                continue;

            await worldService.LoadWorldAsync(project, worldPath).ConfigureAwait(false);
        }

        return project;
    }
    
    private Task SwitchToAsync(IProject? project)
    {
        var current = selectionCoordinator.CurrentProject;
        if (ReferenceEquals(current, project))
            return Task.CompletedTask;;

        selectionCoordinator.SelectProject(project);
        return PersistLastProjectPath(project?.LocalPath ?? new LocalPath());
    }

    private Task PersistLastProjectPath(LocalPath path)
    {
        appStateService.State.LastProjectPath = path;
        return appStateService.SaveAsync();
    }

    private bool _isBusy;
}