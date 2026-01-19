using CommunityToolkit.Mvvm.ComponentModel;
using System.Reactive.Subjects;
using System;


namespace warkbench.Infrastructure;

public partial class SelectionService : ObservableObject, ISelectionService, IDisposable
{
    public SelectionService()
    {
        _subject = new BehaviorSubject<object?>(null);
    }
    
    public void Dispose()
    {
        _subject?.Dispose();
    }
    
    public object? SelectedObject
    {
        get => _selectedObject;
        set
        {
            if (!Equals(_selectedObject, value))
            {
                _selectedObject = value;
                _subject.OnNext(value);
            }
        }
    }

    public IObservable<object?> WhenSelectionChanged => _subject;
    
    private object? _selectedObject;
    private readonly BehaviorSubject<object?> _subject;
}