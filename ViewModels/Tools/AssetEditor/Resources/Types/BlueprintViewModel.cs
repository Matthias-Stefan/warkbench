using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using warkbench.Models;

namespace warkbench.ViewModels;
public sealed class BlueprintViewModel : AssetViewModel, IGraphContainer, IBlueprint
{
    public BlueprintViewModel(GraphModel model)
    {
        Model = model;
        SetName(model.Name);

        foreach (var nodeModel in model.Nodes)
        {
            Nodes.Add(NodeFactory.CreateFromModel(nodeModel));
        }

        foreach (var connection in model.Connections)
        {
            var sourceConnectorModel = connection.Source;
            var targetConnectorModel = connection.Target;

            var sourceConnector = Nodes.OfType<IOutputNodeViewModel>()
                .SelectMany(n => n.Outputs)
                .FirstOrDefault(c => c.Model.Guid == sourceConnectorModel.Guid);
            if (sourceConnector is null)
            {
                continue;
            }
            
            var targetConnector = Nodes.OfType<IInputNodeViewModel>()
                                       .SelectMany(n => n.Inputs)
                                       .FirstOrDefault(c => c.Model.Guid == targetConnectorModel.Guid);
            if (targetConnector is null)
            {
                continue;
            }
            
            var connectionModel = new ConnectionModel
            {
                Guid = System.Guid.NewGuid(),
                Source = sourceConnector.Model,
                Target = targetConnector.Model
            };
            
            Connections.Add(new ConnectionViewModel(connectionModel, sourceConnector, targetConnector));
            sourceConnector.IsConnected = true;
            targetConnector.IsConnected = true;
        }

        Nodes.CollectionChanged += OnNodesChanged;
        Connections.CollectionChanged += OnConnectionsChanged;
    }

    protected override string GetName()
    {
        return Model.Name;
    }

    protected override void SetName(string value)
    {
        Model.Name = value;
    }

    protected override string GetVirtualPath()
    {
        return string.Empty;        
    }

    protected override void SetVirtualPath(string value)
    {
    }

    private void OnNodesChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (NodeViewModel vm in args.OldItems)
            {
                Model.Nodes.Remove(vm.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (NodeViewModel vm in args.NewItems)
            {
                Model.Nodes.Add(vm.Model);    
            }
        }
    }
    
    private void OnConnectionsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectionViewModel connection in args.OldItems)
            {
                Model.Connections.Remove(connection.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectionViewModel connection in args.NewItems)
            {
                Model.Connections.Add(connection.Model);    
            }
        }
    }
    
    public GraphModel Model { get; }
    public ObservableCollection<NodeViewModel> Nodes { get; } = [];
    public ObservableCollection<ConnectionViewModel> Connections { get; } = [];
}