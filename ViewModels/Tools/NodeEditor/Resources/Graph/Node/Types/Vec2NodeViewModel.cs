using Avalonia.Controls;
using Avalonia;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;

public partial class Vec2NodeViewModel : NodeViewModel, IOutputNodeViewModel
{
    public Vec2NodeViewModel(Vec2NodeModel model)
        : base(model)
    {
        BorderColor = NodeBrushes.Vector2D;
        SelectedColor = NodeBrushes.Vector2D;
        
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
    
    public override string DetailsHeader => "Vec2 Node";
    public override object? DetailsIcon => Application.Current?.FindResource("icon_vec2") ?? null;
    
    public double X
    {
        get => Vec2Model.X;
        set
        {
            if (!(Math.Abs(Vec2Model.X - value) > 0.0001))
            {
                return;
            }

            Vec2Model.X = value;
            OnPropertyChanged();
        }
    }
    
    public double Y
    {
        get => Vec2Model.Y;
        set
        {
            if (!(Math.Abs(Vec2Model.Y - value) > 0.0001))
            {
                return;
            }

            Vec2Model.Y = value;
            OnPropertyChanged();
        }
    }
    
    private void OnOutputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                Vec2Model.Outputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                Vec2Model.Outputs.Add(connectorViewModel.Model);    
            }
        }
    }
    
    public Vec2NodeModel Vec2Model => (Model as Vec2NodeModel)!;
    public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];
}