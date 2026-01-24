namespace warkbench.src.basis.interfaces.App;

/// <summary>Provides access to the persisted application state.</summary>
public interface IAppStateService
{
    // TODO: Async!!!
    
    /// <summary>Loads state from persistent storage (or creates defaults).</summary>
    void Load();

    /// <summary>Saves the current state to persistent storage.</summary>
    void Save();
    
    /// <summary>Gets the persisted application state (loaded at startup).</summary>
    IAppState State { get; }
}