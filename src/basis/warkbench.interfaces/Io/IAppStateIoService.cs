using warkbench.src.basis.interfaces.App;

namespace warkbench.src.basis.interfaces.Io;

/// <summary>
/// Provides persistence for the application's internal runtime state
/// (e.g. last opened project, workspace, window layout).
/// </summary>
public interface IAppStateIoService
{
    /// <summary> Loads the previously persisted application state, if any. </summary>
    IAppState? Load();

    /// <summary> Persists the given application state. </summary>
    void Save(IAppState state);
}