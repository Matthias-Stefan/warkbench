using warkbench.src.basis.interfaces.Selection;

namespace warkbench.src.basis.core.Selection;

/// <summary>
/// Describes a change in the active selection, including scope transitions
/// and value changes within a scope.
/// </summary>
internal sealed class ActiveSelectionChangedEventArgs(
    SelectionScope previousScope,
    SelectionScope currentScope,
    object? previousSelection,
    object? currentSelection) : EventArgs
{
    /// <summary>Gets the previously active selection scope.</summary>
    public SelectionScope PreviousScope { get; } = previousScope;

    /// <summary>Gets the currently active selection scope.</summary>
    public SelectionScope CurrentScope { get; } = currentScope;
    
    /// <summary>Gets the previously active selection.</summary>
    public object? PreviousSelection { get; } = previousSelection;

    /// <summary>Gets the currently active selection.</summary>
    public object? CurrentSelection { get; } = currentSelection;
    
    /// <summary>True if the active selection scope has changed.</summary>
    public bool ScopeChanged => PreviousScope != CurrentScope;
}