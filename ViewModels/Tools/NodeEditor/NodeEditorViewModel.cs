using Avalonia.Media;
using Avalonia.Threading;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using warkbench.Infrastructure;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class NodeEditorViewModel : Tool
{
    public NodeEditorViewModel(
        IProjectManager projectManager,
        NodeEditorModel model,
        ISelectionService selectionService)
    {
        Model = model;
        ProjectManager = projectManager;
        _selectionService = selectionService;
        _selectionService.WhenSelectionChanged.Subscribe(OnSelectionChanged);
        PendingConnection = new PendingConnectionViewModel(this);
    }

    // ==================================================
    // ===============   NODE INTERFACE   ===============
    // ==================================================

    /// <summary>
    /// Maps each <see cref="NodeViewModel"/> type to its corresponding
    /// <see cref="NodeModel"/> type. This allows <see cref="NewNodeViewModel{T}"/>
    /// to dynamically create the appropriate model instance for a given view model.
    /// </summary>
    private static readonly Dictionary<Type, Type> NodeTypeMap = new()
    {
        { typeof(BoolNodeViewModel),     typeof(BoolNodeModel) },
        { typeof(ClassNodeViewModel),    typeof(ClassNodeModel) },
        { typeof(FloatNodeViewModel),    typeof(FloatNodeModel) },
        { typeof(Int32NodeViewModel),    typeof(Int32NodeModel) },
        { typeof(Int64NodeViewModel),    typeof(Int64NodeModel) },
        { typeof(PropertyNodeViewModel), typeof(PropertyNodeModel) },
        { typeof(RectNodeViewModel),     typeof(RectNodeModel) },
        { typeof(StringNodeViewModel),   typeof(StringNodeModel) },
        { typeof(TextureNodeViewModel),  typeof(TextureNodeModel) },
        { typeof(Vec2NodeViewModel),     typeof(Vec2NodeModel) },
    };
    
    /// <summary>
    /// Creates a regular (non-blueprint) node of the specified <typeparamref name="T"/> type,
    /// initializes it within the editor model, constructs the corresponding view model,
    /// and adds it to the editor's visual collection.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="NodeViewModel"/> type to create.</typeparam>
    /// <param name="name">The display name assigned to the node.</param>
    /// <param name="description">An optional description for the node.</param>
    /// <param name="location">The initial graph-space position of the node.</param>
    /// <returns>
    /// A fully initialized instance of <typeparamref name="T"/>.
    /// </returns>
    public T NewNodeViewModel<T>(string name = "", string description = "", Avalonia.Point location = new())
        where T : NodeViewModel
    {
        return NewNodeViewModelInternal<T>(nameof(NodeEditorModel.NewNode), name, description, location);
    }

    /// <summary>
    /// Creates a blueprint node of the specified <typeparamref name="T"/> type,
    /// initializes it within the editor model, constructs the corresponding view model,
    /// and adds it to the editor's visual collection.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="NodeViewModel"/> type to create.</typeparam>
    /// <param name="name">The display name assigned to the blueprint node.</param>
    /// <param name="description">An optional description for the blueprint node.</param>
    /// <param name="location">The initial graph-space position of the node.</param>
    /// <returns>
    /// A fully initialized blueprint instance of <typeparamref name="T"/>.
    /// </returns>
    public T NewBlueprintNodeViewModel<T>(string name = "", string description = "", Avalonia.Point location = new())
        where T : NodeViewModel
    {
        return NewNodeViewModelInternal<T>(nameof(NodeEditorModel.NewBlueprintNode), name, description, location);
    }
    
    /// <summary>
    /// Creates a property node of the specified <typeparamref name="T"/> type,
    /// initializes it within the editor model, constructs the corresponding view model,
    /// and adds it to the editor's visual collection.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="NodeViewModel"/> type to create.</typeparam>
    /// <param name="name">The display name assigned to the blueprint node.</param>
    /// <param name="description">An optional description for the blueprint node.</param>
    /// <param name="location">The initial graph-space position of the node.</param>
    /// <returns>
    /// A fully initialized blueprint instance of <typeparamref name="T"/>.
    /// </returns>
    public T NewPropertyNodeViewModel<T>(string name = "", string description = "", Avalonia.Point location = new())
        where T : NodeViewModel
    {
        return NewNodeViewModelInternal<T>(nameof(NodeEditorModel.NewPropertyNode), name, description, location);
    }
    
    /// <summary>
    /// Adds the specified <see cref="NodeViewModel"/> to the editor’s view-model
    /// collection and registers it for connection-related events.
    /// </summary>
    /// <param name="vm">
    /// The node view model to add to the visual representation of the graph.
    /// </param>
    public void AddNodeViewModel(NodeViewModel vm)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            SubscribeConnectionEvents(vm);
            Nodes.Add(vm);
        }
        else
        {
            Dispatcher.UIThread.Post(() =>
            {
                SubscribeConnectionEvents(vm);
                Nodes.Add(vm);
            });
        }
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="Int32NodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddInt32Node(TransformGroup transform)
    {
        AddNodeAtLocation<Int32NodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="Int32NodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddInt32NodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<Int32NodeViewModel>(GetLocation(transform, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="Int64NodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddInt64Node(TransformGroup transform)
    {
        AddNodeAtLocation<Int64NodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="Int64NodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddInt64NodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<Int64NodeViewModel>(GetLocation(transform, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="FloatNodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddFloatNode(TransformGroup transform)
    {
        AddNodeAtLocation<FloatNodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="FloatNodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddFloatNodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<FloatNodeViewModel>(GetLocation(transform, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="StringNodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddStringNode(TransformGroup transform)
    {
        AddNodeAtLocation<StringNodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="StringNodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddStringNodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<StringNodeViewModel>(GetLocation(transform, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="ClassNodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddClassNode(TransformGroup transform)
    {
        AddNodeAtLocation<ClassNodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="ClassNodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddClassNodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<ClassNodeViewModel>(GetLocation(transform, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="TextureNodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddTextureNode(TransformGroup transform)
    {
        AddNodeAtLocation<TextureNodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="TextureNodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddTextureNodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<TextureNodeViewModel>(GetLocation(transform, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="BoolNodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddBoolNode(TransformGroup transform)
    {
        AddNodeAtLocation<BoolNodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="BoolNodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddBoolNodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<BoolNodeViewModel>(GetLocation(transform, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="RectNodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddRectNode(TransformGroup transform)
    {
        AddNodeAtLocation<RectNodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="RectNodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddRectNodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<RectNodeViewModel>(GetLocation(transform, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="Vec2NodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddVec2Node(TransformGroup transform)
    {
        AddNodeAtLocation<Vec2NodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="Vec2NodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddVec2NodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<Vec2NodeViewModel>(GetLocation(transform, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="PropertyNodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddPropertyNode(Tuple<TransformGroup, GraphModel> args)
    {
        AddNodeAtLocation<PropertyNodeViewModel>(GetLocation(args.Item1, new Vector(60, 60)), args.Item2.Name);
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="PropertyNodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddPropertyNodeFromMouse(Tuple<TransformGroup, GraphModel> args)
    {
        AddNodeAtLocation<PropertyNodeViewModel>(GetLocation(args.Item1, LastMousePosition));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Removes the specified node from the editor, including all connections
    /// that reference it, and keeps the underlying model and view-model
    /// collections in sync.
    /// </summary>
    /// <param name="node">
    /// The node view model to remove. If <c>null</c>, the call is ignored.
    /// </param>
    public void RemoveNodeViewModel(NodeViewModel? node)
    {
        if (node is null)
        {
            return;
        }

        var connectionsToRemove = Connections
            .Where(con => con.Source.Node == node || con.Target.Node == node)
            .ToList();

        foreach (var connection in connectionsToRemove)
        {
            connection.Source.IsConnected = false;
            connection.Target.IsConnected = false;

            Model.RemoveConnection(connection.Model);
            Connections.Remove(connection);
        }
        
        Model.RemoveNode(node.Model);
        
        UnsubscribeConnectionEvents(node);
        Nodes.Remove(node);
    }
    
    /// <summary>
    /// Creates a new node of the specified <typeparamref name="T"/> type at the given location.
    /// If the current container represents a blueprint, a blueprint node is created;
    /// otherwise a regular node is instantiated. The resulting node view model is then
    /// added to the editor’s node collection.
    /// </summary>
    /// <typeparam name="T">The concrete <see cref="NodeViewModel"/> type to create.</typeparam>
    /// <param name="location">The graph-space position where the node should appear.</param>
    /// <param name="name">The node name.</param>
    private void AddNodeAtLocation<T>(Avalonia.Point location, string name = "")
        where T : NodeViewModel
    {
        NodeViewModel nodeViewModel = _selectedNodeContainer switch
        {
            IBlueprint _ => NewBlueprintNodeViewModel<T>(location: location),
            IProperty _ => NewPropertyNodeViewModel<T>(location: location),
            _ => NewNodeViewModel<T>(location: location)
        };
        nodeViewModel.Name = name;
    }
    
    /// <summary>
    /// Internal helper used to construct a <see cref="NodeModel"/> and its corresponding
    /// <see cref="NodeViewModel"/> in a generic fashion. This method resolves the model type
    /// mapped to <typeparamref name="T"/>, invokes the appropriate editor-model creation
    /// method (either <c>NewNode</c> or <c>NewBlueprintNode</c>) via reflection, and finally
    /// creates the matching view model instance.
    /// </summary>
    /// <typeparam name="T">
    /// The concrete <see cref="NodeViewModel"/> type to instantiate.
    /// </typeparam>
    /// <param name="editorMethodName">
    /// The name of the <see cref="NodeEditorModel"/> method responsible for creating the
    /// underlying model instance (e.g. <c>"NewNode"</c> or <c>"NewBlueprintNode"</c>).
    /// </param>
    /// <param name="name">The display name assigned to the created node.</param>
    /// <param name="description">An optional description for the node.</param>
    /// <param name="location">The initial graph-space position of the node.</param>
    /// <returns>
    /// A fully constructed instance of <typeparamref name="T"/>, bound to its newly created model.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when no <see cref="NodeModel"/> type is mapped to <typeparamref name="T"/>.
    /// </exception>
    private T NewNodeViewModelInternal<T>(
        string editorMethodName,
        string name,
        string description,
        Avalonia.Point location)
        where T : NodeViewModel
    {
        // Determine the underlying model type for this view model
        var vmType = typeof(T);
        if (!NodeTypeMap.TryGetValue(vmType, out var modelType))
            throw new NotSupportedException($"No model type mapped for {vmType.Name}");

        // Locate the editor model method responsible for creating this type of NodeModel
        var method = typeof(NodeEditorModel)
            .GetMethod(editorMethodName)!
            .MakeGenericMethod(modelType);

        // Create the model instance via reflection
        var model = (NodeModel)method.Invoke(Model, [name, description, location])!;

        // Create the corresponding view model
        var vm = (T)NodeFactory.CreateFromModel(model);

        // Register the new VM inside the editor context
        AddNodeViewModel(vm);

        return vm;
    }
    
    /// <summary>
    /// Converts a screen- or UI-space origin point into graph-space coordinates
    /// by applying the inverse of the provided transform and adding the given offset.
    /// </summary>
    /// <param name="transform">
    /// The transform that maps graph coordinates into screen/UI space. Its inverse
    /// is used to convert the point back into graph space.
    /// </param>
    /// <param name="offset">
    /// A positional offset applied after the transformation, typically used to place
    /// newly created nodes slightly away from the reference point.
    /// </param>
    /// <returns>
    /// The calculated graph-space location representing the transformed origin plus the given offset.
    /// </returns>
    private static Avalonia.Point GetLocation(TransformGroup transform, Vector offset)
    {
        var origin = new Avalonia.Point(0, 0);
        var matrix = transform?.Value ?? Matrix.Identity;
        if (!matrix.TryInvert(out var inverted))
        {
            inverted = Matrix.Identity;    
        }
        var location = inverted.Transform(origin) + offset;
        return location;
    }
    
    
    // ========================================================
    // ===============   CONNECTION INTERFACE   ===============
    // ========================================================
    
    /// <summary>
    /// Creates a new connection between the specified source and target connectors,
    /// updates their connection state, adds the connection to the local collection
    /// and notifies all registered listeners about the new connection.
    /// </summary>
    public Task Connect(ConnectorViewModel source, ConnectorViewModel target)
    {
        var connectionModel = new ConnectionModel
        {
            Guid   = Guid.NewGuid(),
            Source = source.Model,
            Target = target.Model
        };
    
        var connection = new ConnectionViewModel(connectionModel, source, target);
        source.IsConnected = true;
        target.IsConnected = true;
        Connections.Add(connection);
    
        var args = new ConnectionChangedEventArgs(source, target, connection); 
        OnConnected(args);
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Removes the connection associated with the specified connector, updates the
    /// connector state, removes it from the local collection and notifies all
    /// registered listeners about the disconnection.
    /// </summary>
    public Task Disconnect(ConnectorViewModel connector)
    {
        var connection = Connections.First(con => con.Source == connector || con.Target == connector);
        connection.Source.IsConnected = false;
        connection.Target.IsConnected = false;
        Connections.Remove(connection);
    
        var args = new ConnectionChangedEventArgs(connection.Source, connection.Target, connection); 
        OnDisconnected(args);
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Command handler that disconnects the connection associated with the given connector.
    /// </summary>
    [RelayCommand]
    private Task OnDisconnect(ConnectorViewModel connector) => Disconnect(connector);
    
    /// <summary>
    /// Raises the internal "connected" notification with the given event arguments.
    /// </summary>
    /// <param name="args">Details about the created connection.</param>
    protected virtual void OnConnected(ConnectionChangedEventArgs args)
    {
        _connected?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the internal "disconnected" notification with the given event arguments.
    /// </summary>
    /// <param name="args">Details about the removed connection.</param>
    protected virtual void OnDisconnected(ConnectionChangedEventArgs args)
    {
        _disconnected?.Invoke(this, args);
    }
    
    /// <summary>
    /// Determines whether the specified handler has already been added to the
    /// multicast delegate to avoid duplicate registrations.
    /// </summary>
    /// <param name="action">The current multicast delegate instance.</param>
    /// <param name="handler">The handler to check for.</param>
    /// <returns>
    /// <see langword="true"/> if the handler is already registered; otherwise <see langword="false"/>.
    /// </returns>
    private bool IsHandlerAlreadyAdded(
        Action<object, ConnectionChangedEventArgs>? action, 
        Action<object, ConnectionChangedEventArgs> handler)
    {
        if (action == null)
        {
            return false;    
        }
    
        return action
            .GetInvocationList()
            .Any(d => d.Method == handler.Method && d.Target == handler.Target);
    }
    
    /// <summary>
    /// Subscribes the given node's connection handlers to the internal connection
    /// notification callbacks, if they are not already registered.
    /// </summary>
    /// <param name="node">The node whose connection handlers should be registered.</param>
    private void SubscribeConnectionEvents(NodeViewModel node)
    {
        if (!IsHandlerAlreadyAdded(_connected, node.HandleConnected))
        {
            _connected += node.HandleConnected;
        }

        if (!IsHandlerAlreadyAdded(_disconnected, node.HandleDisconnected))
        {
            _disconnected += node.HandleDisconnected;    
        }
    }

    /// <summary>
    /// Unsubscribes the given node's connection handlers from the internal
    /// connection notification callbacks.
    /// </summary>
    /// <param name="node">The node whose connection handlers should be removed.</param>
    private void UnsubscribeConnectionEvents(NodeViewModel node)
    {
        _connected     -= node.HandleConnected;
        _disconnected  -= node.HandleDisconnected;
    }
    
    /// <summary>
    /// Delegate invoked whenever a connection between two connectors has been created.
    /// Nodes subscribe their connection handlers to this callback.
    /// </summary>
    private Action<object, ConnectionChangedEventArgs>? _connected;

    /// <summary>
    /// Delegate invoked whenever an existing connection between two connectors has been removed.
    /// Nodes subscribe their disconnect handlers to this callback.
    /// </summary>
    private Action<object, ConnectionChangedEventArgs>? _disconnected;
    
    
    // ============================================
    // ===============   INTERNAL   ===============
    // ============================================
    
    /// <summary>
    /// Clears all node and connection view models from the editor’s visual state.
    /// This does not modify the underlying model; it only resets the ViewModel layer.
    /// </summary>
    private void Clear()
    {
        Nodes.Clear();
        Connections.Clear();
    }
    
    /// <summary>
    /// Handles changes to the external graph selection. When a new 
    /// <see cref="IGraphContainer"/> is selected, this method updates the editor’s 
    /// internal state to reflect the nodes and connections of the selected container.
    /// It resets the currently selected node, rebinds node and connection collections,
    /// subscribes connection event handlers for all nodes, and notifies the UI of 
    /// the updated properties.
    /// </summary>
    /// <param name="obj">
    /// The newly selected graph container, or another object that is ignored.
    /// </param>
    private void OnSelectionChanged(object? obj)
    {
        if (obj is not IGraphContainer graphContainer)
        {
            return;    
        }
        
        _selectedNode = null;
        _selectedNodeContainer = graphContainer;
        Nodes = graphContainer.Nodes;
        foreach (var node in graphContainer.Nodes)
        {
            SubscribeConnectionEvents(node);
        }
        Connections = graphContainer.Connections;
        
        OnPropertyChanged(nameof(SelectedNode));
        OnPropertyChanged(nameof(IsEnabled));
        OnPropertyChanged(nameof(Nodes));
        OnPropertyChanged(nameof(Connections));
    }
    
    /// <summary>
    /// Performs a tree/forest layout on the current Nodes / Connections.
    /// </summary>
    private void Sort(Dictionary<NodeViewModel, NodeRect> nodeRectsMap)
    {
        // 1) Build parent/child maps in a pass
        var allNodes = nodeRectsMap.Keys.ToList();

        var nodeChildrenMap = allNodes.ToDictionary(
            n => n,
            _ => new List<NodeViewModel>());

        var nodeParentMap = new Dictionary<NodeViewModel, NodeViewModel>();

        foreach (var connection in Connections)
        {
            var source = connection.Source.Node;
            var target = connection.Target.Node;

            if (source is null || target is null)
                continue;

            // child (source) => parent (target)
            if (nodeChildrenMap.TryGetValue(target, out var children))
            {
                children.Add(source);
            }

            if (!nodeParentMap.ContainsKey(source))
            {
                nodeParentMap[source] = target;
            }
        }

        // 2) Identify leaves
        var leaves = allNodes
            .Where(n =>
                n is IOutputNodeViewModel ||
                n is IInputNodeViewModel { Inputs.Count: 0 })
            .ToList();

        var currentIt = leaves.ToHashSet();

        const double xOffset = 100.0;
        const double yOffset = 20.0;

        // 3) Walk up from the leaves and layout subtrees
        while (true)
        {
            var parentsCurrentIt = new HashSet<NodeViewModel>();

            foreach (var node in currentIt)
            {
                if (nodeParentMap.TryGetValue(node, out var parent))
                {
                    parentsCurrentIt.Add(parent);
                }
            }

            if (parentsCurrentIt.Count == 0)
                break;

            foreach (var parent in parentsCurrentIt)
            {
                if (!nodeChildrenMap.TryGetValue(parent, out var children) || children.Count == 0)
                    continue;

                // Sort children by current Y position
                var orderedChildren = children
                    .Where(child => nodeRectsMap.ContainsKey(child))
                    .OrderBy(child => nodeRectsMap[child].Rect.Y)
                    .ToList();

                if (orderedChildren.Count == 0)
                    continue;

                double y = 0.0;
                var groupRect = new NodeRect();

                // 3a) Place child subtrees vertically below each other
                foreach (var child in orderedChildren)
                {
                    var childRect = nodeRectsMap[child];
                    var rect = childRect.Rect;

                    // We move the subtree so that its upper left corner is at (0, y).
                    var delta = new Vector(
                        -rect.X,
                        y - rect.Y);

                    childRect.Move(delta);

                    // Increase y for the next child
                    y += childRect.Rect.Height + yOffset;

                    groupRect.Merge(childRect);
                }

                // 3b) Place parent to the right of children, vertically centered
                var center = groupRect.Rect.Center;
                var parentRect = nodeRectsMap[parent];

                var parentDelta = new Vector(
                    groupRect.Rect.Width + xOffset - parent.Location.X,
                    center.Y - parent.Size.Height / 2.0 - parent.Location.Y);

                parentRect.Move(parentDelta);
                groupRect.Merge(parentRect);

                // 3c) Update subtree rect for parent
                nodeRectsMap[parent] = groupRect;
            }

            currentIt.Clear();
            currentIt.UnionWith(parentsCurrentIt);
        }

        // 4) Arrange several trees (Forest) next to each other
        var roots = allNodes
            .Where(n => !nodeParentMap.ContainsKey(n))
            .ToList();

        // Optional: Sort roots, e.g. by Y (or by name)
        roots = roots
            .OrderBy(r => nodeRectsMap[r].Rect.Y)
            .ToList();

        const double treeSpacing = 200.0;
        double currentX = 0.0;

        foreach (var root in roots)
        {
            var treeRect = nodeRectsMap[root];

            // Move the tree so that its upper left corner is at (currentX, 0)
            var delta = new Vector(
                currentX - treeRect.Rect.X,
                0.0 - treeRect.Rect.Y);

            treeRect.Move(delta);

            currentX += treeRect.Rect.Width + treeSpacing;
        }
    }
    
    /// <summary>
    /// Recalculates the layout of all nodes by computing subtree bounding boxes
    /// and arranging them into a structured forest layout. Existing node positions
    /// are used as the initial base rectangles before performing a full bottom-up
    /// layout pass.
    /// </summary>
    [RelayCommand]
    private Task OnSortNodes()
    {
        // Generate base rectangles from current node positions
        var nodeRectsMap = Nodes
            .ToHashSet()
            .ToDictionary(node => node, node => new NodeRect(node));

        Sort(nodeRectsMap);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Moves the viewport so that the graph content is brought into view.
    /// If at least one node exists, the viewport is centered on the first node;
    /// otherwise it falls back to the origin.
    /// </summary>
    [RelayCommand]
    private Task OnBringIntoView()
    {
        // Use the location of the first node as a reference point,
        // or fall back to the origin if the graph is empty.
        ViewportLocation = Nodes.FirstOrDefault()?.Location ?? new Avalonia.Point(0, 0);
        OnPropertyChanged(nameof(ViewportLocation));
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Indicates whether the node editor is currently active.
    /// The editor is enabled as soon as a graph container is selected.
    /// </summary>
    public bool IsEnabled => _selectedNodeContainer is not null;

    /// <summary>
    /// Represents a connection that is currently being created by the user
    /// but has not yet been finalized.
    /// </summary>
    public PendingConnectionViewModel PendingConnection { get; }

    /// <summary>
    /// Collection of all node view models currently visible in the editor.
    /// </summary>
    public ObservableCollection<NodeViewModel> Nodes { get; set; } = [];

    /// <summary>
    /// Collection of all connections between nodes in the current graph.
    /// </summary>
    public ObservableCollection<ConnectionViewModel> Connections { get; set; } = [];

    /// <summary>
    /// Stores the last known mouse position in editor coordinates.
    /// </summary>
    public Avalonia.Point LastMousePosition { get; set; } = new Point(0, 0);
    
    /// <summary>
    /// Gets or sets the current viewport location in graph-space coordinates.
    /// This value defines the top-left origin of the visible editor area
    /// and is used for panning and programmatic navigation.
    /// </summary>
    public Avalonia.Point ViewportLocation { get; set; } = new Point(0, 0);
    
    /// <summary>
    /// Gets or sets the currently selected node.
    /// Setting this property also propagates the selection
    /// to the global selection service.
    /// </summary>
    public NodeViewModel? SelectedNode
    {
        get => _selectedNode;
        set
        {
            if (value is null)
            {
                return;    
            }

            Console.WriteLine(value);
            _selectedNode = value;
            _selectionService.SelectedObject = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Provides access to the project manager that owns the current graph.
    /// </summary>
    public IProjectManager ProjectManager { get; }

    /// <summary>
    /// Underlying model backing this node editor view model.
    /// </summary>
    private NodeEditorModel Model { get; }
    
    /// <summary>
    /// Central selection service used to synchronize selections
    /// across different editor components.
    /// </summary>
    private readonly ISelectionService _selectionService;
    
    /// <summary>
    /// Currently selected node instance, if any.
    /// </summary>
    private NodeViewModel? _selectedNode = null;
    
    /// <summary>
    /// Currently active graph container whose nodes and connections
    /// are displayed and edited.
    /// </summary>
    private IGraphContainer? _selectedNodeContainer = null;
    
   /// <summary>
    /// Represents a bounding box for a logical subtree of nodes. A <see cref="NodeRect"/> 
    /// tracks all nodes that belong to the same subtree and computes a unified rectangle 
    /// that tightly encloses all of them. The rect automatically updates whenever nodes 
    /// are added, merged, or moved.
    /// 
    /// <para>
    /// This structure is used by the layout algorithm to treat entire subtrees as 
    /// movable blocks. Calling <see cref="Move"/> shifts all contained nodes together, 
    /// preserving their relative spatial configuration.
    /// </para>
    /// </summary>
    private sealed class NodeRect
    {
        /// <summary>
        /// Initializes a new <see cref="NodeRect"/> containing an optional initial node.
        /// The bounding rectangle is computed immediately.
        /// </summary>
        public NodeRect(NodeViewModel? node = null)
        {
            if (node is not null)
            {
                _nodes.Add(node);
            }

            Update();
        }

        /// <summary>
        /// Adds a single node to the subtree. If the node is newly added,
        /// the bounding rectangle is recalculated.
        /// </summary>
        public void Add(NodeViewModel node)
        {
            if (_nodes.Add(node))
            {
                Update();
            }
        }

        /// <summary>
        /// Merges another <see cref="NodeRect"/> into this one by importing all nodes 
        /// from the other subtree. The resulting bounding rectangle covers both sets.
        /// </summary>
        public void Merge(NodeRect other)
        {
            foreach (var node in other._nodes)
            {
                _nodes.Add(node);
            }

            Update();
        }

        /// <summary>
        /// Moves all nodes contained in this subtree by the given vector. 
        /// Node positions are updated directly, and the bounding rectangle 
        /// is recalculated afterwards.
        /// </summary>
        public void Move(Avalonia.Vector delta)
        {
            if (delta == default)
                return;

            foreach (var node in _nodes)
            {
                node.Location += delta;
            }

            Update();
        }

        /// <summary>
        /// Recomputes the bounding rectangle so that it tightly encloses
        /// all nodes in the subtree. If the subtree is empty, a zero-sized rect is used.
        /// </summary>
        public void Update()
        {
            if (_nodes.Count == 0)
            {
                Rect = new Rect(0, 0, 0, 0);
                return;
            }

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            foreach (var n in _nodes)
            {
                minX = Math.Min(minX, n.Location.X);
                minY = Math.Min(minY, n.Location.Y);
                maxX = Math.Max(maxX, n.Location.X + n.Size.Width);
                maxY = Math.Max(maxY, n.Location.Y + n.Size.Height);
            }

            Rect = new Avalonia.Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Gets all nodes currently included in this subtree. 
        /// </summary>
        public IReadOnlyCollection<NodeViewModel> Nodes => _nodes;

        /// <summary>
        /// Gets the bounding rectangle that encloses all nodes in the subtree.
        /// This value updates whenever the subtree contents or positions change.
        /// </summary>
        public Avalonia.Rect Rect { get; private set; } = new Avalonia.Rect(0, 0, 0, 0);

        /// <summary>
        /// An optional transform container for future extensions. 
        /// Not used directly by the layout algorithm, but available for UI rendering layers.
        /// </summary>
        public TransformGroup Translate { get; private set; } = new TransformGroup();

        private readonly HashSet<NodeViewModel> _nodes = [];
    }
}