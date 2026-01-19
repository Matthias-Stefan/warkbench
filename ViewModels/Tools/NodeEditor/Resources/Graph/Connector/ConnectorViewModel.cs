using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.Models;


namespace warkbench.ViewModels;

public partial class ConnectorViewModel : ObservableObject
{
    public ConnectorViewModel(ConnectorModel model, NodeViewModel node)
    {
        Model = model;
        Node = node;
    }
    
    public string Title
    {
        get => Model.Title;
        set
        {
            if (Model.Title == value)
            {
                return;
            }

            Model.Title = value;
            OnPropertyChanged();
        }
    }
    
    public Avalonia.Point Anchor
    {
        get => Model.Anchor;
        set
        {
            if (Model.Anchor == value)
            {
                return;
            }

            Model.Anchor = value;
            OnPropertyChanged();
        }
    }
    
    public bool IsConnected
    {
        get => Model.IsConnected;
        set
        {
            if (Model.IsConnected == value)
            {
                return;
            }

            Model.IsConnected = value;
            OnPropertyChanged();
        }
    }
    
    public NodeViewModel Node { get; }
    public ConnectorModel Model { get; }
}