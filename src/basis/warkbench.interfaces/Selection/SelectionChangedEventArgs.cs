namespace warkbench.src.basis.interfaces.Selection;

/// <summary>Describes a change in selection state.</summary>
public sealed class SelectionChangedEventArgs<T>(
    IReadOnlyList<T> previousItems,
    IReadOnlyList<T> currentItems,
    T? previousPrimary,
    T? currentPrimary,
    bool scopeChanged) : EventArgs
{
    /// <summary>Gets the selection snapshot before the change.</summary>
    public IReadOnlyList<T> PreviousItems { get; } = previousItems;

    /// <summary>Gets the selection snapshot after the change.</summary>
    public IReadOnlyList<T> CurrentItems { get; } = currentItems;

    /// <summary>Gets the primary selection before the change.</summary>
    public T? PreviousPrimary { get; } = previousPrimary;

    /// <summary>Gets the primary selection after the change.</summary>
    public T? CurrentPrimary { get; } = currentPrimary;

    /// <summary>True if the primary selection reference has changed.</summary>
    public bool PrimaryChanged =>
        !EqualityComparer<T?>.Default.Equals(PreviousPrimary, CurrentPrimary);

    /// <summary>True if the selection items changed.</summary>
    public bool ItemsChanged =>
        PreviousItems.Count != CurrentItems.Count ||
        !PreviousItems.SequenceEqual(CurrentItems);

    /// <summary>True if the active selection scope has changed.</summary>
    public bool ScopeChanged => scopeChanged;
}