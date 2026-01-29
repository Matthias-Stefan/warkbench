namespace warkbench.src.basis.interfaces.Selection;

/// <summary>
/// Provides data for a selection-changing event, including the primary selection
/// and the scope in which the change occurs.
/// </summary>
public class SelectionChangingEventArgs<T>(T? currentPrimary, SelectionScope currentScope) : EventArgs
{
    /// <summary>Gets the primary selection after the change.</summary>
    public T? CurrentPrimary { get; } = currentPrimary;

    /// <summary>Gets the scope in which the selection change occurred.</summary>
    public SelectionScope CurrentScope { get; } = currentScope;
}