namespace warkbench.src.basis.interfaces.Selection;

/// <summary>
/// Represents a scoped subscription to active selection changes.
/// </summary>
public interface ISelectionSubscription : IDisposable
{
    /// <summary>Raised before the active selection changes.</summary>
    event SelectionChangingEventHandler<object>? Changing;
    
    /// <summary>Raised when the active selection changes for this subscription.</summary>
    event SelectionChangedEventHandler<object>? Changed;
}