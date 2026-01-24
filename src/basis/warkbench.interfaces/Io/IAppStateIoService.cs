using warkbench.src.basis.interfaces.App;

namespace warkbench.src.basis.interfaces.Io;

public interface IAppStateIoService : IIoService<IAppState>
{
    /// <summary>Loads the application state from the default application data location.</summary>
    IAppState? Load();

    /// <summary>Asynchronously loads the application state from the default application data location.</summary>
    Task<IAppState?> LoadAsync();

    /// <summary>Saves the application state to the default application data location.</summary>
    void Save(IAppState state);

    /// <summary>Asynchronously saves the application state to the default application data location.</summary>
    Task SaveAsync(IAppState state);

}