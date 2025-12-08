using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class PropertyNodeViewModel : NodeViewModel, IInputNodeViewModel
{
    public PropertyNodeViewModel(PropertyNodeModel model) 
        : base(model)
    {
        BorderColor = NodeBrushes.Property;
        SelectedColor = NodeBrushes.Property;
        
        Inputs.CollectionChanged += OnInputsChanged;
        if (model.Inputs.Count == 0)
        {
            Inputs.Add(new ConnectorViewModel(new ConnectorModel{ Guid = System.Guid.NewGuid() }, this));    
        }
        else
        {
            foreach (var connector in model.Inputs)
            {
                Inputs.Add(new ConnectorViewModel(connector, this));
            }
        }
    }
    
    public override void HandleConnected(object? sender, ConnectionChangedEventArgs? args)
    {
        if (args is null)
        {
            return;
        }

        var sourceNode = args.SourceConnector.Node;
        if (!_subscriptions.ContainsKey(args.Connection))
        {
            sourceNode.PropertyChanged += OnSourceNodePropertyChanged;
            _subscriptions[args.Connection] = sourceNode;
        }
        
        args.TargetConnector.Title = sourceNode.Name;
        if (Inputs.Last().IsConnected)
        {
            Inputs.Add(new ConnectorViewModel(new ConnectorModel{ Guid = System.Guid.NewGuid() }, this));    
        }
    }

    public override void HandleDisconnected(object? sender, ConnectionChangedEventArgs? args)
    {
        if (args is null)
        {
            return;
        }
        
        if (_subscriptions.TryGetValue(args.Connection, out var sourceNode))
        {
            sourceNode.PropertyChanged -= OnSourceNodePropertyChanged;
            _subscriptions.Remove(args.Connection);
        }
        
        Inputs.Remove(args.TargetConnector);
    }



    private void OnSourceNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not NodeViewModel node || e.PropertyName != nameof(NodeViewModel.Name))
        {
            return;
        }

        foreach (var kvp in _subscriptions.Where(kvp => Equals(kvp.Key.Source.Node, node)))
        {
            kvp.Key.Target.Title = node.Name;
        }
    }

    private void OnInputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                PropertyModel.Inputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                PropertyModel.Inputs.Add(connectorViewModel.Model);    
            }
        }
    }

    public PropertyNodeModel PropertyModel => (Model as PropertyNodeModel)!;
    public ObservableCollection<ConnectorViewModel> Inputs { get; } = [];
    private readonly Dictionary<ConnectionViewModel, INotifyPropertyChanged> _subscriptions = [];
}