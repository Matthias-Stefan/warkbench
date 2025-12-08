using System.Collections.ObjectModel;
using System.Collections.Specialized;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class PropertyNodeViewModel : NodeViewModel, IOutputNodeViewModel
{
    public PropertyNodeViewModel(PropertyNodeModel model)
        : base(model)
    {
        BorderColor = NodeBrushes.Property;
        SelectedColor = NodeBrushes.Property;

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

    public object Value
    {
        get => PropertyModel.Value;
        set
        {
            if (PropertyModel.Value == value)
            {
                return;
            }
            
            PropertyModel.Value = value;
            OnPropertyChanged();
        }
    }
    
    private void OnOutputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                PropertyModel.Outputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                PropertyModel.Outputs.Add(connectorViewModel.Model);    
            }
        }
    }
    
    public PropertyNodeModel PropertyModel => (Model as PropertyNodeModel)!;
    public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];
}