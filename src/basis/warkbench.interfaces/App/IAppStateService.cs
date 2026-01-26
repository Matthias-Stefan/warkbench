namespace warkbench.src.basis.interfaces.App;

/// <summary>Provides access to the persisted application state.</summary>
public interface IAppStateService
{
    /// <summary>Loads state from persistent storage (or creates defaults).</summary>
    Task LoadAsync();

    /// <summary>Saves the current state to persistent storage.</summary>
    Task SaveAsync();
    
    /// <summary>Gets the persisted application state (loaded at startup).</summary>
    IAppState State { get; }
}