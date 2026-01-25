namespace warkbench.src.basis.interfaces.Selection;

/// <summary>Handles selection change notifications.</summary>
public delegate void SelectionChangedEventHandler<T>(object? sender, SelectionChangedEventArgs<T> args);