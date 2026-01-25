using System.Collections.ObjectModel;

namespace warkbench.src.basis.core.Selection;

/// <summary>Lightweight selection container that tracks selected items and a primary item.</summary>
internal sealed class SelectionSet<T>
{
    public SelectionSet()
    {
        Items = new ReadOnlyObservableCollection<T>(_items);
    }

    public void Select(T item)
    {
        if (item is null)
            return;

        if (_items.Count == 1 && EqualityComparer<T>.Default.Equals(_items[0], item))
            return;

        _items.Clear();
        _items.Add(item);
        Primary = item;
    }

    public void Deselect(T item)
    {
        if (item is null)
            return;

        var idx = _items.IndexOf(item);
        if (idx < 0)
            return;

        _items.RemoveAt(idx);

        if (Primary is not null && EqualityComparer<T>.Default.Equals(Primary, item))
            Primary = _items.Count > 0 ? _items[^1] : default;
    }

    public void Select(IEnumerable<T> items)
    {
        _items.Clear();

        var seen = new HashSet<T>(EqualityComparer<T>.Default);
        foreach (var item in items)
        {
            if (item is not null && seen.Add(item))
                _items.Add(item);
        }

        Primary = _items.Count > 0 ? _items[^1] : default;
    }

    public void Deselect(IEnumerable<T> items)
    {
        var removeList = items.Where(i => i is not null)
                              .ToHashSet(EqualityComparer<T>.Default);

        if (removeList.Count == 0 || _items.Count == 0)
            return;

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

        if (Primary is not null && !Contains(Primary))
            Primary = _items.Count > 0 ? _items[^1] : default;
    }

    public bool Contains(T item) => _items.Contains(item, EqualityComparer<T>.Default);

    public ReadOnlyObservableCollection<T> Items { get; }

    public T? Primary
    {
        get => _primary;
        private set
        {
            if (EqualityComparer<T>.Default.Equals(_primary, value))
                return;

            _primary = value;
        }
    }

    private readonly ObservableCollection<T> _items = [];
    private T? _primary;
}
