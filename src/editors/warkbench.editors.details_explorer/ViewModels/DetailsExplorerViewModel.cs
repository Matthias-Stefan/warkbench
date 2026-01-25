using Dock.Model.Mvvm.Controls;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Selection;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.editors.details_explorer.ViewModels;

public class DetailsExplorerViewModel : Tool, IDisposable
{
    public DetailsExplorerViewModel(
        ILogger logger,
        ISelectionCoordinator selectionCoordinator
        )
    {
        _logger = logger;
        _selectionCoordinator = selectionCoordinator;
        
        _selectionSubscription = selectionCoordinator.Subscribe();
        _selectionSubscription.Changed += OnSelectionChanged;
    }
    
    public void Dispose()
    {
        _selectionSubscription.Changed -= OnSelectionChanged;
        _selectionSubscription.Dispose();
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs<object> e)
    {
        int x = 5;
    }
    
    private void OnProjectSelectionChanged(object? sender, SelectionChangedEventArgs<IProject> e)
    {
        int x = 5;
    }

    private void OnWorldSelectionChanged(object? sender, SelectionChangedEventArgs<IWorld> e)
    {
        int x = 5;
    }
    
    public object? Selected { get; private set; }
    private readonly ISelectionSubscription _selectionSubscription;
    
    private readonly ILogger _logger;
    private readonly ISelectionCoordinator _selectionCoordinator;
}