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

        _selectionCoordinator.SelectionChanging += OnSelectionChanging;
        _selectionCoordinator.SelectionChanged += OnSelectionChanged;
    }

    public void Dispose()
    {
        _selectionCoordinator.SelectionChanging -= OnSelectionChanging;
        _selectionCoordinator.SelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanging(object? sender, SelectionChangingEventArgs<object> e)
    {
        var currentRelevant = _selectionScopes.Contains(e.CurrentScope);
        if (!currentRelevant)
            return;
        
        Changing?.Invoke(this, e);
    }
    
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs<object> e)
    {
        var currentRelevant = _selectionScopes.Contains(e.CurrentScope);
        if (!currentRelevant)
            return;

        Changed?.Invoke(this, e);
    }
    
    public event SelectionChangingEventHandler<object>? Changing;
    public event SelectionChangedEventHandler<object>? Changed;
    
    private readonly SelectionCoordinator _selectionCoordinator;
    private readonly IReadOnlySet<SelectionScope> _selectionScopes;
}