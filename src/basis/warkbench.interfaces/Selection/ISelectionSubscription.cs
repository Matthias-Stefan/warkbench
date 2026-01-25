namespace warkbench.src.basis.interfaces.Selection;

/// <summary>
/// Represents a scoped subscription to active selection changes.
/// </summary>
public interface ISelectionSubscription : IDisposable
{
    /// <summary>Gets the current active selection scope.</summary>
    SelectionScope ActiveScope { get; }

    /// <summary>Gets the active selection if the scope is relevant to this subscription.</summary>
    object? ActiveSelection { get; }
    
    /// <summary>Raised when the active selection changes for this subscription.</summary>
    event SelectionChangedEventHandler<object>? Changed;
}