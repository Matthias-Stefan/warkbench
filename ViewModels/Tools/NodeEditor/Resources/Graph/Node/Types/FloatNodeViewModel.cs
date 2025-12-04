using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Media;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class FloatNodeViewModel : NodeViewModel, IOutputNodeViewModel
{
    public FloatNodeViewModel(FloatNodeModel model)
        : base(model)
    {
        BorderColor = new SolidColorBrush(Colors.Goldenrod);
        SelectedColor = new SolidColorBrush(Colors.Goldenrod);

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

    public float Value
    {
        get => FloatModel.Value;
        set
        {
            if (!(Math.Abs(FloatModel.Value - value) > 0.0001f))
            {
                return;    
            }
            
            FloatModel.Value = value;
            OnPropertyChanged();
        }
    }
    
    private void OnOutputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                FloatModel.Outputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                FloatModel.Outputs.Add(connectorViewModel.Model);    
            }
        }
    }
    
    public FloatNodeModel FloatModel => (Model as FloatNodeModel)!;
    public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];
}