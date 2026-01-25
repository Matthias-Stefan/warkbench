using System.ComponentModel;
using System.Runtime.CompilerServices;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Selection;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Selection;

public class SelectionCoordinator : ISelectionCoordinator
{
    public ISelectionSubscription Subscribe(params SelectionScope[] scopes)
    {
        var normalizeScopes = NormalizeScopes(scopes);
        return new SelectionSubscription(normalizeScopes, this);
    }

    public void SelectProject(IProject project)
    {
        _projectSelectionSet.Select(project);
        SetActive(SelectionScope.Project, project);
    }

    public void SelectWorld(IWorld world)
    {
        _worldSelectionSet.Select(world);
        SetActive(SelectionScope.World, world);
    }
    
    private void SetActive(SelectionScope scope, object? selection)
    {
        var prevScope = ActiveScope;
        var prevSelection = ActiveSelection;

        var scopeChanged = prevScope != scope;
        var selectionChanged = !ReferenceEquals(prevSelection, selection);

        if (!scopeChanged && !selectionChanged)
            return;
        
        ActiveScope = scope;
        ActiveSelection = selection;

        OnPropertyChanged(nameof(ActiveScope));
        OnPropertyChanged(nameof(ActiveSelection));
        OnPropertyChanged(nameof(CurrentProject));
        OnPropertyChanged(nameof(CurrentWorld));
        
        ActiveSelectionChanged?.Invoke(
            this,
            new ActiveSelectionChangedEventArgs(prevScope, scope, prevSelection, selection));
    }
    
    private static IReadOnlySet<SelectionScope> NormalizeScopes(SelectionScope[] scopes)
    {
        if (scopes.Length == 0)
        {
            return Enum.GetValues<SelectionScope>()
                .Where(s => s != SelectionScope.None)
                .ToHashSet();
        }

        return scopes.Where(s => s != SelectionScope.None)
            .ToHashSet();
    }
    
    public object? ActiveSelection { get; private set; }
    public SelectionScope ActiveScope { get; private set; } = SelectionScope.None; 
    public IProject? CurrentProject => _projectSelectionSet.Primary;
    public IWorld? CurrentWorld => _worldSelectionSet.Primary;
    
    internal event EventHandler<ActiveSelectionChangedEventArgs>? ActiveSelectionChanged;
    
    private readonly SelectionSet<IProject> _projectSelectionSet = new();
    private readonly SelectionSet<IWorld> _worldSelectionSet = new();
    
    // ---- INotifyPropertyChanged ----

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}