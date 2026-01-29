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
        var currentScope = SelectedScope;
        var currentProject = _projectSelectionSet.Primary;
        
        if (ReferenceEquals(currentProject, project))
            return;
        
        SelectionChanging?.Invoke(this, new SelectionChangingEventArgs<object>(currentProject, currentScope));
        
        _projectSelectionSet.Select(project);
        
        SelectedScope = SelectionScope.Project;
        SelectedItem = project;
        
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<object>(
            currentProject, currentScope, SelectedItem, SelectedScope));
    }

    public void SelectWorld(IWorld world)
    {
        var currentScope = SelectedScope;
        var currentWorld = _worldSelectionSet.Primary;
        
        if (ReferenceEquals(currentWorld, world))
            return;
        
        SelectionChanging?.Invoke(this, new SelectionChangingEventArgs<object>(currentWorld, currentScope));

        _worldSelectionSet.Select(world);
        
        SelectedScope = SelectionScope.World;
        SelectedItem = world;
        
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<object>(
            currentWorld, currentScope, SelectedItem, SelectedScope));
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
    
    public SelectionScope SelectedScope { get; private set; } = SelectionScope.None;
    public object? SelectedItem { get; private set; }
    public IProject? LastSelectedProject => _projectSelectionSet.Primary;
    public IWorld? LastSelectedWorld => _worldSelectionSet.Primary;
    
    internal event EventHandler<SelectionChangingEventArgs<object>>? SelectionChanging;
    internal event EventHandler<SelectionChangedEventArgs<object>>? SelectionChanged;
    
    private readonly SelectionSet<IProject> _projectSelectionSet = new();
    private readonly SelectionSet<IWorld> _worldSelectionSet = new();
    
    // ---- INotifyPropertyChanged ----

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}