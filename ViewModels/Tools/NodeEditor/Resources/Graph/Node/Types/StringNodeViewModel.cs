using Avalonia.Controls;
using Avalonia;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class StringNodeViewModel : NodeViewModel, IOutputNodeViewModel
{
    public StringNodeViewModel(StringNodeModel model)
        : base(model)
    {
        BorderColor = NodeBrushes.String;
        SelectedColor = NodeBrushes.String;

        Outputs.CollectionChanged += OnOutputsChanged;
        if (model.Outputs.Count == 0)
        {
            Outputs.Add(new ConnectorViewModel(new ConnectorModel{ Guid  = System.Guid.NewGuid() }, this));    
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
    
    public override string DetailsHeader => "String Node";
    public override object? DetailsIcon => Application.Current?.FindResource("icon_string") ?? null;

    public string Value
    {
        get => StringModel.Value;
        set
        {
            if (StringModel.Value == value)
            {
                return;
            }

            StringModel.Value = value;
            OnPropertyChanged();
        }
    }
    
    private void OnOutputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                StringModel.Outputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                StringModel.Outputs.Add(connectorViewModel.Model);    
            }
        }
    }

    public StringNodeModel StringModel => (Model as StringNodeModel)!;

    public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];
}