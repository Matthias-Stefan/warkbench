using warkbench.src.basis.interfaces.Selection;

namespace warkbench.src.basis.core.Selection;

internal sealed class SelectionSubscription : ISelectionSubscription
{
    public SelectionSubscription(
        IReadOnlySet<SelectionScope> selectionScopes,
        SelectionCoordinator selectionCoordinator
        )
    {
        _selectionScopes = selectionScopes;
        _selectionCoordinator = selectionCoordinator;

        _selectionCoordinator.ActiveSelectionChanged += OnActiveSelectionChanged;
    }
    
    public void Dispose()
        => _selectionCoordinator.ActiveSelectionChanged -= OnActiveSelectionChanged;

    private void OnActiveSelectionChanged(object? sender, ActiveSelectionChangedEventArgs e)
    {
        if (!_selectionScopes.Contains(_selectionCoordinator.ActiveScope))
            return;
        
        var prevRelevant = _selectionScopes.Contains(e.PreviousScope);
        var currRelevant = _selectionScopes.Contains(e.CurrentScope);

        // Only notify if this subscription cares about either side of the transition.
        if (!prevRelevant && !currRelevant)
            return;

        // For subscribers that don't care about a scope, we publish "no selection".
        var previousItems = prevRelevant && e.PreviousSelection is not null
            ? new[] { e.PreviousSelection }
            : Array.Empty<object>();

        var currentItems = currRelevant && e.CurrentSelection is not null
            ? new[] { e.CurrentSelection }
            : Array.Empty<object>();

        var args = new SelectionChangedEventArgs<object>(
            previousItems,
            currentItems,
            prevRelevant ? e.PreviousSelection : null,
            currRelevant ? e.CurrentSelection : null,
            e.ScopeChanged);
        
        Changed?.Invoke(this, args);
    }
    
    public object? ActiveSelection =>
        _selectionScopes.Contains(_selectionCoordinator.ActiveScope) 
            ? _selectionCoordinator.ActiveSelection 
            : null;

    public SelectionScope ActiveScope =>
        _selectionScopes.Contains(_selectionCoordinator.ActiveScope)
            ? _selectionCoordinator.ActiveScope
            : SelectionScope.None;
    
    public event SelectionChangedEventHandler<object>? Changed;
    
    private readonly SelectionCoordinator _selectionCoordinator;
    private readonly IReadOnlySet<SelectionScope> _selectionScopes;
}