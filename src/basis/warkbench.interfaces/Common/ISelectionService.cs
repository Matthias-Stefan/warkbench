using System.Collections.ObjectModel;
using System.ComponentModel;

namespace warkbench.src.basis.interfaces.Common;

/// <summary>
/// Manages primary and multi-selection state.
/// Implements INotifyPropertyChanged to support UI bindings for the Primary property.
/// </summary>
public interface ISelectionService<T> : INotifyPropertyChanged
{
    /// <summary>Selects a single item and clears the current selection.</summary>
    void Select(T item);
    
    /// <summary>Removes an item from the current selection.</summary>
    void Deselect(T item);
    
    /// <summary>Replaces the current selection with the given items.</summary>
    void Select(IEnumerable<T> items);
    
    /// <summary>Removes the given items from the current selection.</summary>
    void Deselect(IEnumerable<T> items);
    
    /// <summary>Returns true if the item is currently selected.</summary>
    bool Contains(T item);
    
    /// <summary>Gets the primary selected item.</summary>
    T? Primary { get; }
    
    /// <summary>
    /// Gets all currently selected items as a reactive collection.
    /// </summary>
    ReadOnlyObservableCollection<T> Items { get; }
    
    /// <summary>Raised when the selection state changes with detailed delta information.</summary>
    event EventHandler<SelectionChangedEventArgs<T>> SelectionChanged;
}

/// <summary>Describes a change in selection state.</summary>
public sealed class SelectionChangedEventArgs<T>(
    IReadOnlyList<T> previous, 
    IReadOnlyList<T> current,
    T? previousPrimary, 
    T? currentPrimary) : EventArgs
{
    public IReadOnlyList<T> Previous { get; } = previous;
    public IReadOnlyList<T> Current { get; } = current;
    public T? PreviousPrimary { get; } = previousPrimary;
    public T? CurrentPrimary { get; } = currentPrimary;

    /// <summary>True if the primary selection reference has changed.</summary>
    public bool PrimaryChanged =>
        !EqualityComparer<T?>.Default.Equals(PreviousPrimary, CurrentPrimary);

    /// <summary>
    /// True if the count or the set of selected items has changed.
    /// Note: This is a shallow check based on count or snapshot.
    /// </summary>
    public bool ItemsChanged => !Previous.SequenceEqual(Current);
}