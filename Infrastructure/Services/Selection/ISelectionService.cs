using System;


namespace warkbench.Infrastructure;

public interface ISelectionService
{
    object? SelectedObject { get; set; }
    IObservable<object?> WhenSelectionChanged { get; }
}