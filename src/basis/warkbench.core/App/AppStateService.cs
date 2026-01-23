using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;

namespace warkbench.src.basis.core.App;

public class AppStateService(IAppStateIoService appStateIo, ILogger logger) : IAppStateService
{
    public void Load()
    {
        try
        {
            var loadedState = appStateIo.Load();
            State = loadedState ?? new AppState();

            logger.Info("[AppStateService] Application state loaded.");
        }
        catch (Exception ex)
        {
            logger.Error($"[AppStateService] Failed to load application state: {ex.Message}");
            State = new AppState();
        }
    }

    public void Save()
    {
        try
        {
            appStateIo.Save(State);
            logger.Info("[AppStateService] Application state saved.");
        }
        catch (Exception ex)
        {
            logger.Error($"[AppStateService] Failed to save application state: {ex.Message}");
            throw;
        }
    }
    
    public IAppState State { get; private set; } = new AppState();
}