using Avalonia.Controls;
using Avalonia;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class Int64NodeViewModel : NodeViewModel, IOutputNodeViewModel
{
    public Int64NodeViewModel(Int64NodeModel model)
        : base(model)
    {
        BorderColor = NodeBrushes.Int64;
        SelectedColor = NodeBrushes.Int64;
        
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
    
    public override string DetailsHeader => "Int64 Node";
    public override object? DetailsIcon => Application.Current?.FindResource("icon_int64") ?? null;
    
    public Int64 Value
    {
        get => IntModel.Value;
        set
        {
            if (IntModel.Value == value)
            {
                return;
            }

            IntModel.Value = value;
            OnPropertyChanged();
        }
    }
    
    private void OnOutputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                IntModel.Outputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                IntModel.Outputs.Add(connectorViewModel.Model);    
            }
        }
    }
    
    public Int64NodeModel IntModel => (Model as Int64NodeModel)!;
    public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];
}