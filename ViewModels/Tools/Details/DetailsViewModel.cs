using Dock.Model.Mvvm.Controls;
using System;
using warkbench.Infrastructure;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class DetailsViewModel : Tool
{
    public DetailsViewModel(DetailsModel model, ISelectionService selectionService)
    {
        Model = model;
        OnSelectionChanged(new object());
        selectionService.WhenSelectionChanged.Subscribe(OnSelectionChanged);
    }
    
    private void OnSelectionChanged(object? obj)
    {
        SelectedObject = obj;
        OnPropertyChanged(nameof(SelectedObject));
    }

    public DetailsModel Model { get; }
    public object? SelectedObject { get; set; }
}
