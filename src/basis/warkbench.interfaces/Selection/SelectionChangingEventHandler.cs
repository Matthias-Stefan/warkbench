namespace warkbench.src.basis.interfaces.Selection;

/// <summary>Handles selection changing notifications.</summary>
public delegate void SelectionChangingEventHandler<T>(object? sender, SelectionChangingEventArgs<T> args);