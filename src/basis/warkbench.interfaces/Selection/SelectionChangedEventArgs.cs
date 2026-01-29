namespace warkbench.src.basis.interfaces.Selection;

/// <summary>Describes a change in selection state.</summary>
public sealed class SelectionChangedEventArgs<T>(
    T? previousPrimary,
    SelectionScope previousScope,
    T? currentPrimary,
    SelectionScope currentScope) : EventArgs
{
    /// <summary>Gets the previous primary selection, if any.</summary>
    public T?  PreviousPrimary { get; } =  previousPrimary;
    
    /// <summary>Gets the scope of the previous selection.</summary>
    public SelectionScope PreviousScope { get; } = previousScope;
    
    /// <summary>Gets the current primary selection, if any.</summary>
    public T? CurrentPrimary { get; } =  currentPrimary;
    
    /// <summary>Gets the scope of the current selection.</summary>
    public SelectionScope CurrentScope { get; } = currentScope;
}