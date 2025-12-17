using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using warkbench.Brushes;
using warkbench.ViewModels;


namespace warkbench.Models;
public class NodeEditorModel
{
    // ==================================================
    // ===============   NODE INTERFACE   ===============
    // ==================================================

    /// <summary>
    /// Creates a new node of the specified type, initializes it with the provided metadata,
    /// adds it to the graph, and notifies listeners that the set of nodes has changed.
    /// </summary>
    /// <typeparam name="T">The concrete node type to create.</typeparam>
    /// <param name="name">An optional display name for the node.</param>
    /// <param name="description">An optional description associated with the node.</param>
    /// <param name="location">The initial position of the node within the graph.</param>
    /// <param name="internalGraph"></param>
    /// <returns>The newly created node instance.</returns>
    public T NewNode<T>(
        string name = "", 
        string description = "", 
        Avalonia.Point location = new(),
        GraphModel? internalGraph = null) 
        where T : NodeModel, new()
    {
        var node = new T
        {
            Guid = System.Guid.NewGuid(),
            Name = name,
            Description = description,
            Location = location,
            NodeHeaderBrushType = NodeHeaderBrushType.None,
            InternalGraph = internalGraph?.DeepClone() ?? null
        };

        _nodes.Add(node);
        OnNodesChanged(new NodesChangedEventArgs 
        {
            Type = NodeChangeType.New,
            Node = node
        });
        
        return node;
    }

    /// <summary>
    /// Creates a new blueprint node of the specified type, initializes it with the
    /// provided metadata, adds it to the graph, and notifies listeners that the 
    /// set of nodes has changed.
    /// </summary>
    /// <typeparam name="T">The concrete node type to create.</typeparam>
    /// <param name="name">An optional display name for the blueprint node.</param>
    /// <param name="description">An optional description associated with the node.</param>
    /// <param name="location">The initial position of the node within the graph.</param>
    /// <param name="internalGraph"></param>
    /// <returns>The newly created blueprint node instance.</returns>
    public T NewBlueprintNode<T>(
        string name = "", 
        string description = "", 
        Avalonia.Point location = new(),
        GraphModel? internalGraph = null)
        where T : NodeModel, new()
    {
        var node = new T
        {
            Guid = System.Guid.NewGuid(),
            Name = name,
            Description = description,
            Location = location,
            NodeHeaderBrushType = NodeHeaderBrushType.Blueprint,
            InternalGraph = internalGraph?.DeepClone() ?? null
        };

        _nodes.Add(node);
        OnNodesChanged(new NodesChangedEventArgs 
        {
            Type = NodeChangeType.New,
            Node = node
        });
        
        return node;
    }

    /// <summary>
    /// Creates a new property node of the specified type, initializes it with the
    /// provided metadata, adds it to the graph, and notifies listeners that the 
    /// set of nodes has changed.
    /// </summary>
    /// <typeparam name="T">The concrete node type to create.</typeparam>
    /// <param name="name">An optional display name for the blueprint node.</param>
    /// <param name="description">An optional description associated with the node.</param>
    /// <param name="location">The initial position of the node within the graph.</param>
    /// <param name="internalGraph"></param>
    /// <returns>The newly created blueprint node instance.</returns>
    public T NewPropertyNode<T>(
        string name = "", 
        string description = "", 
        Avalonia.Point location = new(),
        GraphModel? internalGraph = null) 
        where T : NodeModel, new()
    {
        var node = new T
        {
            Guid = System.Guid.NewGuid(),
            Name = name,
            Description = description,
            Location = location,
            NodeHeaderBrushType = NodeHeaderBrushType.Property,
            InternalGraph = internalGraph?.DeepClone() ?? null
        };

        _nodes.Add(node);
        OnNodesChanged(new NodesChangedEventArgs 
        {
            Type = NodeChangeType.New,
            Node = node
        });
        
        return node;
    }
    
    /// <summary>
    /// Adds the specified node to the graph and notifies listeners
    /// if the set of nodes has changed.
    /// </summary>
    /// <param name="node">The node to add.</param>
    /// <returns>
    /// <see langword="true"/> if the node was added successfully;
    /// <see langword="false"/> if the node was already present.
    /// </returns>
    public bool AddNode(NodeModel node)
    {
        var result = _nodes.Add(node);
        if (result)
        {
            OnNodesChanged(new NodesChangedEventArgs 
            {
                Type = NodeChangeType.Added,
                Node = node
            });
        }

        return result;
    }

    /// <summary>
    /// Removes the specified node from the graph and notifies listeners
    /// if the set of nodes has changed.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    /// <returns>
    /// <see langword="true"/> if the node was removed;
    /// <see langword="false"/> if the node was not found.
    /// </returns>
    public bool RemoveNode(NodeModel node)
    {
        var result = _nodes.Remove(node);
        if (result)
        {
            OnNodesChanged(new NodesChangedEventArgs 
            {
                Type = NodeChangeType.Removed,
                Node = node
            });
        }

        return result;
    }

    /// <summary>
    /// Removes all nodes from the graph and notifies listeners
    /// that the set of nodes has changed.
    /// </summary>
    public void ClearNodes()
    {
        _nodes.Clear();
        OnNodesChanged(new NodesChangedEventArgs 
        {
            Type = NodeChangeType.Cleared,
        });
    }

    /// <summary>
    /// Attempts to retrieve the actual stored node instance that is considered
    /// equal to the specified lookup node.
    /// </summary>
    /// <param name="equalNode">
    /// A node used for equality comparison to find the corresponding stored node.
    /// </param>
    /// <param name="actualNode">
    /// When this method returns <see langword="true"/>, contains the matching node;
    /// otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a matching node was found; otherwise <see langword="false"/>.
    /// </returns>
    public bool TryGetNode(NodeModel equalNode, [MaybeNullWhen(false)] out NodeModel actualNode)
    {
        return _nodes.TryGetValue(equalNode, out actualNode);
    }
    
    /// <summary>
    /// Occurs when the set of nodes in the graph has changed.
    /// </summary>
    public event EventHandler? NodesChanged;
    
    /// <summary>
    /// Gets a read-only view of all nodes currently contained in the graph.
    /// </summary>
    public IReadOnlyCollection<NodeModel> Nodes => _nodes;
    
    
    // ========================================================
    // ===============   CONNECTION INTERFACE   ===============
    // ========================================================
    
    /// <summary>
    /// Creates a new connection between the specified source and target connectors,
    /// adds it to the graph, and notifies listeners that the connection set has changed.
    /// </summary>
    /// <param name="source">The connector acting as the source of the connection.</param>
    /// <param name="target">The connector acting as the target of the connection.</param>
    /// <returns>The newly created <see cref="ConnectionModel"/>.</returns>
    public ConnectionModel NewConnection(ConnectorModel source, ConnectorModel target)
    {
        var connection = new ConnectionModel
        {
            Guid = System.Guid.NewGuid(),
            Source = source,
            Target = target
        };
        
        _connections.Add(connection);
        OnConnectionsChanged(new ConnectionsChangedEventArgs 
        {
            Type = ConnectionChangeType.New,
            Connection = connection
        });
        
        return connection;
    }

    /// <summary>
    /// Adds the specified connection to the graph and notifies listeners
    /// if the connection set has changed.
    /// </summary>
    /// <param name="connection">The connection to add.</param>
    /// <returns>
    /// <see langword="true"/> if the connection was added;
    /// <see langword="false"/> if it was already present.
    /// </returns>
    public bool AddConnection(ConnectionModel connection)
    {
        var result = _connections.Add(connection);
        if (result)
        {
            OnConnectionsChanged(new ConnectionsChangedEventArgs 
            {
                Type = ConnectionChangeType.Added,
                Connection = connection
            });
        }

        return result;
    }

    /// <summary>
    /// Removes the specified connection from the graph and notifies listeners
    /// if the connection set has changed.
    /// </summary>
    /// <param name="connection">The connection to remove.</param>
    /// <returns>
    /// <see langword="true"/> if the connection was removed;
    /// <see langword="false"/> if it was not found.
    /// </returns>
    public bool RemoveConnection(ConnectionModel connection)
    {
        var result = _connections.Remove(connection);
        if (result)
        {
            OnConnectionsChanged(new ConnectionsChangedEventArgs 
            {
                Type = ConnectionChangeType.Removed,
                Connection = connection
            });
        }

        return result;
    }

    /// <summary>
    /// Removes all connections from the graph and notifies listeners
    /// that the connection set has been cleared.
    /// </summary>
    public void ClearConnections()
    {
        _connections.Clear();
        OnConnectionsChanged(new ConnectionsChangedEventArgs() 
        {
            Type = ConnectionChangeType.Cleared,
        });
    }
    
    /// <summary>
    /// Attempts to retrieve the stored connection instance that is considered equal
    /// to the provided lookup connection.
    /// </summary>
    /// <param name="equalConnection">
    /// A connection instance used for equality comparison.
    /// </param>
    /// <param name="actualConnection">
    /// When the method returns <see langword="true"/>, contains the matching stored connection;
    /// otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a matching connection was found; otherwise <see langword="false"/>.
    /// </returns>
    public bool TryGetConnection(ConnectionModel equalConnection, [MaybeNullWhen(false)] out ConnectionModel actualConnection)
    {
        return _connections.TryGetValue(equalConnection, out actualConnection);
    }
    

    // ============================================
    // ===============   INTERNAL   ===============
    // ============================================
    
    protected virtual void OnNodesChanged(NodesChangedEventArgs e) => NodesChanged?.Invoke(this, e);
    protected virtual void OnConnectionsChanged(ConnectionsChangedEventArgs e) => NodesChanged?.Invoke(this, e);
    
    private readonly HashSet<NodeModel> _nodes = [];
    private readonly HashSet<ConnectionModel> _connections = [];
}