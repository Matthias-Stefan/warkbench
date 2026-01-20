using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using warkbench.src.basis.interfaces.Common;

namespace warkbench.src.basis.core.Common;

/// <summary>
/// Implementierung des Selection-Services für Avalonia/MVVM.
/// Nutzt ObservableCollections für automatische UI-Updates.
/// </summary>
public class SelectionService<T> : ISelectionService<T>, INotifyPropertyChanged
{
    public SelectionService()
    {
        Items = new ReadOnlyObservableCollection<T>(_items);
    }

    public void Select(T item)
    {
        if (item == null) 
            return;
        if (Primary is not null && Primary.Equals(item) && _items.Count == 1)
            return;

        var prev = Snapshot();
        var prevPrimary = Primary;

        _items.Clear();
        _items.Add(item);
        Primary = item;

        RaiseChanged(prev, Snapshot(), prevPrimary, Primary);
    }

    public void Deselect(T item)
    {
        if (item == null) 
            return;
        
        var idx = _items.IndexOf(item);
        if (idx == -1) 
            return;

        var prev = Snapshot();
        var prevPrimary = Primary;

        _items.RemoveAt(idx);

        // Falls das gelöschte Item das Primary war, rücke nach
        if (Primary is not null && Primary.Equals(item))
        {
            Primary = _items.Count > 0 ? _items[^1] : default;
        }

        RaiseChanged(prev, Snapshot(), prevPrimary, Primary);
    }

    public void Select(IEnumerable<T> items)
    {
        var prev = Snapshot();
        var prevPrimary = Primary;

        _items.Clear();
        
        var seen = new HashSet<T>(EqualityComparer<T>.Default);
        foreach (var item in items)
        {
            if (item is not null && seen.Add(item))
            {
                _items.Add(item);
            }
        }

        Primary = _items.Count > 0 ? _items[^1] : default;
        RaiseChanged(prev, Snapshot(), prevPrimary, Primary);
    }

    public void Deselect(IEnumerable<T> items)
    {
        var removeList = items.Where(i => i is not null).ToHashSet();
        if (removeList.Count == 0 || _items.Count == 0) 
            return;

        var prev = Snapshot();
        var prevPrimary = Primary;
        var changed = false;

        for (var i = _items.Count - 1; i >= 0; i--)
        {
            if (!removeList.Contains(_items[i])) 
                continue;
            _items.RemoveAt(i);
            changed = true;
        }

        if (!changed) 
            return;

        // Primary Logik korrigieren
        if (Primary is not null && !Contains(Primary))
        {
            Primary = _items.Count > 0 ? _items[^1] : default;
        }

        RaiseChanged(prev, Snapshot(), prevPrimary, Primary);
    }

    public bool Contains(T item) => _items.Contains(item, EqualityComparer<T>.Default);

    private void RaiseChanged(IReadOnlyList<T> prev, IReadOnlyList<T> curr, T? prevPrimary, T? currPrimary)
    {
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<T>(prev, curr, prevPrimary, currPrimary));
    }

    private IReadOnlyList<T> Snapshot() => _items.ToArray();

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public ReadOnlyObservableCollection<T> Items { get; }

    public T? Primary
    {
        get => _primary;
        private set
        {
            if (EqualityComparer<T>.Default.Equals(_primary, value)) 
                return;
            _primary = value;
            OnPropertyChanged();
        }
    }
    
    public event EventHandler<SelectionChangedEventArgs<T>>? SelectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private readonly ObservableCollection<T> _items = [];
    private T? _primary;
}