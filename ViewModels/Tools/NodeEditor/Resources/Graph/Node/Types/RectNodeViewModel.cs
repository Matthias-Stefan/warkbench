using Avalonia.Controls;
using Avalonia;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class RectNodeViewModel : NodeViewModel, IOutputNodeViewModel
{
    public RectNodeViewModel(RectNodeModel model)
        : base(model)
    {
        BorderColor = NodeBrushes.Rectangle;
        SelectedColor = NodeBrushes.Rectangle;
        
        Outputs.CollectionChanged += OnOutputsChanged;
        if (model.Outputs.Count == 0)
        {
            Outputs.Add(new ConnectorViewModel(new ConnectorModel{ Guid = System.Guid.NewGuid() }, this));    
        }
        else
        {
            foreach (var connector in model.Outputs)
            {
                Outputs.Add(new ConnectorViewModel(connector, this));
            }
        }
    }
    
    public override void HandleConnected(object? sender, ConnectionChangedEventArgs? args)
    {
    }

    public override void HandleDisconnected(object? sender, ConnectionChangedEventArgs? args)
    {
    }
    
    public override string DetailsHeader => "Rect Node";
    public override object? DetailsIcon => Application.Current?.FindResource("icon_rect_node") ?? null;
    
    public int X
    {
        get => RectModel.X;
        set
        {
            if (RectModel.X == value)
            {
                return;
            }

            RectModel.X = value;
            OnPropertyChanged();
        }
    }
    
    public int Y
    {
        get => RectModel.Y;
        set
        {
            if (RectModel.Y == value)
            {
                return;
            }

            RectModel.Y = value;
            OnPropertyChanged();
        }
    }
    
    public int Width
    {
        get => RectModel.Width;
        set
        {
            if (RectModel.Width == value)
            {
                return;
            }

            RectModel.Width = value;
            OnPropertyChanged();
        }
    }
    
    public int Height
    {
        get => RectModel.Height;
        set
        {
            if (RectModel.Height == value)
            {
                return;
            }

            RectModel.Height = value;
            OnPropertyChanged();
        }
    }
    
    private void OnOutputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                RectModel.Outputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                RectModel.Outputs.Add(connectorViewModel.Model);    
            }
        }
    }
    
    public RectNodeModel RectModel => (Model as RectNodeModel)!;
    public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];
}
