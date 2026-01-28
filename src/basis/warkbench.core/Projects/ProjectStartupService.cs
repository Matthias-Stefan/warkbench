using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Logger;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.core.Projects;

public class ProjectStartupService(
    IAppStateService appStateService,
    ILogger logger,
    IProjectService projectService,
    IProjectSession projectSession
    ) : IProjectStartupService
{
    public async Task RunAsync()
    {
        try
        {
            // 1) Load persisted app state
            await appStateService.LoadAsync();

            // 2) Try auto-open last project
            if (appStateService.State.LastProjectPath is not { } lastProjectPath)
                return;

            logger.Info<ProjectStartupService>($"Auto-loading last project: {lastProjectPath.Value}");
            await projectSession.OpenAsync(lastProjectPath, ProjectLoadMode.StartupWorld);
        }
        catch (Exception e)
        {
            logger.Warn<ProjectStartupService>("Stored application state references a project that no longer exists. " +
                                               "Continuing startup without restoring the previous project.");
            appStateService.State.LastProjectPath = null;
            await appStateService.SaveAsync();
        }
    }
}