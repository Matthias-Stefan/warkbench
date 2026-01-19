using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;

public abstract partial class NodeViewModel : ObservableObject, IDetailsHeader
{
    protected NodeViewModel(NodeModel model)
    {
        _model = model;
    }

    public abstract void HandleConnected(object? sender, ConnectionChangedEventArgs? args);
    public abstract void HandleDisconnected(object? sender, ConnectionChangedEventArgs? args);
    public abstract string DetailsHeader { get; }
    public abstract object? DetailsIcon { get; }
    
    [ObservableProperty] 
    private NodeModel _model;

    public string Title => Model.Title;

    public string Name
    {
        get => Model.Name;
        set
        {
            if (Model.Name == value)
            {
                return;
            }

            Model.Name = value;
            OnPropertyChanged();
        }
    }
    
    public string Description
    {
        get => Model.Description;
        set
        {
            if (Model.Description == value)
            {
                return;
            }

            Model.Description = value;
            OnPropertyChanged();
        }
    }
    
    public Avalonia.Point Location
    {
        get => Model.Location;
        set
        {
            if (Model.Location == value)
            {
                return;
            }

            Model.Location = value;
            OnPropertyChanged();
        }
    }

    public Avalonia.Size Size
    {
        get => _size;
        set =>  SetProperty(ref _size, value);
    }

    private Avalonia.Size _size;
    
    public NodeHeaderBrushType NodeHeaderBrushType
    {
        get => Model.NodeHeaderBrushType;
        set
        {
            if (Model.NodeHeaderBrushType == value)
            {
                return;
            }

            Model.NodeHeaderBrushType = value;
            OnPropertyChanged(nameof(NodeHeaderBrushType));
        }
    }
    
    [ObservableProperty] 
    private SolidColorBrush _borderColor = new SolidColorBrush(Colors.Gray);
    
    [ObservableProperty] 
    private SolidColorBrush _selectedColor = new SolidColorBrush(Colors.White);

    public GraphViewModel? InternalGraph { get; set; } = null;

    public bool IsContentVisible
    {
        get => NodeHeaderBrushType == NodeHeaderBrushType.None && _isContentVisible;
        set => SetProperty(ref _isContentVisible, value);
    }

    private bool _isContentVisible = true;
}