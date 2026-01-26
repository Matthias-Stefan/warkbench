using warkbench.src.basis.interfaces.App;

namespace warkbench.src.basis.interfaces.Io;

public interface IAppStateIoService : IIoService<IAppState>
{
    /// <summary>Asynchronously loads the application state from the default application data location.</summary>
    Task<IAppState?> LoadAsync();

    /// <summary>Asynchronously saves the application state to the default application data location.</summary>
    Task SaveAsync(IAppState state);
}