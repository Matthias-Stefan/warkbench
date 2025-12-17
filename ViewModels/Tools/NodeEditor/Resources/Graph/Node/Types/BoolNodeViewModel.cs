using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class BoolNodeViewModel : NodeViewModel, IOutputNodeViewModel
{
    public BoolNodeViewModel(BoolNodeModel model)
        : base(model)
    {
        BorderColor = NodeBrushes.Bool;
        SelectedColor = NodeBrushes.Bool;

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

    public override string DetailsHeader => "Bool Node";
    public override object? DetailsIcon => Application.Current?.FindResource("icon_bool_node") ?? null;

    public bool Value
    {
        get => BoolModel.Value;
        set
        {
            if (BoolModel.Value == value)
            {
                return;
            }
            
            BoolModel.Value = value;
            OnPropertyChanged();
        }
    }
    
    private void OnOutputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                BoolModel.Outputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                BoolModel.Outputs.Add(connectorViewModel.Model);    
            }
        }
    }
    
    public BoolNodeModel BoolModel => (Model as BoolNodeModel)!;
    public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];
}