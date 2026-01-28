using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Logger;

namespace warkbench.src.basis.core.App;

public class AppStateService(IAppStateIoService appStateIo, ILogger logger) : IAppStateService
{
    public async Task LoadAsync()
    {
        try
        {
            var loadedState = await appStateIo.LoadAsync();
            State = loadedState ?? new AppState();

            logger.Info<AppStateService>("Application state loaded.");
        }
        catch (Exception ex)
        {
            logger.Error<AppStateService>($"Failed to load application state: {ex.Message}");
            State = new AppState();
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            await appStateIo.SaveAsync(State);
            logger.Info<AppStateService>("Application state saved.");
        }
        catch (Exception ex)
        {
            logger.Error<AppStateService>($"Failed to save application state: {ex.Message}");
            throw;
        }
    }
    
    public IAppState State { get; private set; } = new AppState();
}