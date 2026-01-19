using Avalonia.Controls;
using Avalonia;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System;
using warkbench.Models;


namespace warkbench.ViewModels;

public class GraphViewModel : AssetViewModel, IGraphContainer, IDisposable
{
    public GraphViewModel(GraphModel model)
    {
        Model = model;
        SetName(model.Name);

        BuildFromModel();

        Model.Nodes.CollectionChanged += OnModelNodesChanged;
        Model.Connections.CollectionChanged += OnModelConnectionsChanged;

        Nodes.CollectionChanged += OnVmNodesChanged;
        Connections.CollectionChanged += OnVmConnectionsChanged;
    }
    
    public void Dispose()
    {
        Model.Nodes.CollectionChanged -= OnModelNodesChanged;
        Model.Connections.CollectionChanged -= OnModelConnectionsChanged;

        Nodes.CollectionChanged -= OnVmNodesChanged;
        Connections.CollectionChanged -= OnVmConnectionsChanged;
    }
    
    private void BuildFromModel()
    {
        _syncingFromModel = true;
        try
        {
            Nodes.Clear();
            Connections.Clear();

            foreach (var nodeModel in Model.Nodes)
            {
                var vm = NodeFactory.CreateFromModel(nodeModel);
                Nodes.Add(vm);
            }

            foreach (var connectionModel in Model.Connections)
            {
                TryCreateConnectionVmFromModel(connectionModel);
            }
        }
        finally
        {
            _syncingFromModel = false;
        }
    }

    private void OnVmNodesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_syncingFromModel) return;

        _syncingFromVm = true;
        try
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Model.Nodes.Clear();
                return;
            }

            if (e.OldItems is not null)
            {
                foreach (NodeViewModel vm in e.OldItems)
                {
                    Model.Nodes.Remove(vm.Model);
                }
            }

            if (e.NewItems is not null)
            {
                foreach (NodeViewModel vm in e.NewItems)
                {
                    if (!Model.Nodes.Contains(vm.Model))
                        Model.Nodes.Add(vm.Model);
                }
            }
        }
        finally
        {
            _syncingFromVm = false;
        }
    }

    private void OnVmConnectionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_syncingFromModel) return;

        _syncingFromVm = true;
        try
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Model.Connections.Clear();
                return;
            }

            if (e.OldItems is not null)
            {
                foreach (ConnectionViewModel vm in e.OldItems)
                {
                    Model.Connections.Remove(vm.Model);
                }
            }

            if (e.NewItems is not null)
            {
                foreach (ConnectionViewModel vm in e.NewItems)
                {
                    if (!Model.Connections.Contains(vm.Model))
                        Model.Connections.Add(vm.Model);
                }
            }
        }
        finally
        {
            _syncingFromVm = false;
        }
    }

    private void OnModelNodesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_syncingFromVm) return;

        _syncingFromModel = true;
        try
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Nodes.Clear();
                Connections.Clear();
                return;
            }

            if (e.OldItems is not null)
            {
                foreach (NodeModel m in e.OldItems)
                {
                    var vm = Nodes.FirstOrDefault(x => ReferenceEquals(x.Model, m) || x.Model.Guid == m.Guid);
                    if (vm is not null)
                        Nodes.Remove(vm);

                    RemoveDanglingConnectionsForNode(m);
                }
            }

            if (e.NewItems is not null)
            {
                foreach (NodeModel m in e.NewItems)
                {
                    if (Nodes.Any(x => ReferenceEquals(x.Model, m) || x.Model.Guid == m.Guid))
                        continue;

                    Nodes.Add(NodeFactory.CreateFromModel(m));
                }

                foreach (var c in Model.Connections)
                {
                    EnsureConnectionVmExists(c);
                }
            }
        }
        finally
        {
            _syncingFromModel = false;
        }
    }

    private void OnModelConnectionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_syncingFromVm) return;

        _syncingFromModel = true;
        try
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Connections.Clear();
                return;
            }

            if (e.OldItems is not null)
            {
                foreach (ConnectionModel m in e.OldItems)
                {
                    var vm = Connections.FirstOrDefault(x => ReferenceEquals(x.Model, m) || x.Model.Guid == m.Guid);
                    if (vm is not null)
                        Connections.Remove(vm);

                    MarkEndpointsDisconnected(m);
                }
            }

            if (e.NewItems is not null)
            {
                foreach (ConnectionModel m in e.NewItems)
                {
                    EnsureConnectionVmExists(m);
                }
            }
        }
        finally
        {
            _syncingFromModel = false;
        }
    }

    private void EnsureConnectionVmExists(ConnectionModel connectionModel)
    {
        if (Connections.Any(x => ReferenceEquals(x.Model, connectionModel) || x.Model.Guid == connectionModel.Guid))
            return;

        TryCreateConnectionVmFromModel(connectionModel);
    }

    private void TryCreateConnectionVmFromModel(ConnectionModel connectionModel)
    {
        var sourceConnectorModel = connectionModel.Source;
        var targetConnectorModel = connectionModel.Target;

        var sourceConnector = Nodes.OfType<IOutputNodeViewModel>()
            .SelectMany(n => n.Outputs)
            .FirstOrDefault(c => c.Model.Guid == sourceConnectorModel.Guid);

        var targetConnector = Nodes.OfType<IInputNodeViewModel>()
            .SelectMany(n => n.Inputs)
            .FirstOrDefault(c => c.Model.Guid == targetConnectorModel.Guid);

        if (sourceConnector is null || targetConnector is null)
            return;

        sourceConnector.IsConnected = true;
        targetConnector.IsConnected = true;

        Connections.Add(new ConnectionViewModel(connectionModel, sourceConnector, targetConnector));
    }

    private void RemoveDanglingConnectionsForNode(NodeModel nodeModel)
    {
        var nodeVm = Nodes.FirstOrDefault(x => x.Model.Guid == nodeModel.Guid);
        var connectorGuids = nodeVm switch
        {
            IInputNodeViewModel invm and IOutputNodeViewModel outvm
                => invm.Inputs.Select(x => x.Model.Guid).Concat(outvm.Outputs.Select(x => x.Model.Guid)).ToHashSet(),
            IInputNodeViewModel invmOnly
                => invmOnly.Inputs.Select(x => x.Model.Guid).ToHashSet(),
            IOutputNodeViewModel outvmOnly
                => outvmOnly.Outputs.Select(x => x.Model.Guid).ToHashSet(),
            _ => null
        };

        if (connectorGuids is null)
            return;

        var toRemove = Connections
            .Where(c => connectorGuids.Contains(c.Model.Source.Guid) || connectorGuids.Contains(c.Model.Target.Guid))
            .ToList();

        foreach (var c in toRemove)
        {
            Connections.Remove(c);
        }
    }

    private void MarkEndpointsDisconnected(ConnectionModel m)
    {
        var src = Nodes.OfType<IOutputNodeViewModel>()
            .SelectMany(n => n.Outputs)
            .FirstOrDefault(c => c.Model.Guid == m.Source.Guid);

        var dst = Nodes.OfType<IInputNodeViewModel>()
            .SelectMany(n => n.Inputs)
            .FirstOrDefault(c => c.Model.Guid == m.Target.Guid);

        if (src is not null) src.IsConnected = false;
        if (dst is not null) dst.IsConnected = false;
    }
    
    
    public GraphModel Model { get; protected set; }

    public ObservableCollection<NodeViewModel> Nodes { get; } = [];
    
    public ObservableCollection<ConnectionViewModel> Connections { get; } = [];

    protected override string GetName() => Model.Name;

    protected sealed override void SetName(string value) => Model.Name = value;

    protected override string GetVirtualPath() => string.Empty;

    protected override void SetVirtualPath(string value) { }
    
    public override string DetailsHeader => GetName();
    
    public override object? DetailsIcon { get; } = null;

    private bool _syncingFromModel;
    private bool _syncingFromVm;
}