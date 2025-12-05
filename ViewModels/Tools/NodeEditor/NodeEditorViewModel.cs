using Avalonia.Media;
using Avalonia;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using warkbench.Infrastructure;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class NodeEditorViewModel : Tool
{
    public NodeEditorViewModel(
        NodeEditorModel model,
        ISelectionService selectionService)
    {
        Model = model;
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
        { typeof(BoolNodeViewModel),    typeof(BoolNodeModel) },
        { typeof(ClassNodeViewModel),   typeof(ClassNodeModel) },
        { typeof(FloatNodeViewModel),   typeof(FloatNodeModel) },
        { typeof(IntNodeViewModel),     typeof(IntNodeModel) },
        { typeof(RectNodeViewModel),    typeof(RectNodeModel) },
        { typeof(StringNodeViewModel),  typeof(StringNodeModel) },
        { typeof(TextureNodeViewModel), typeof(TextureNodeModel) },
        { typeof(Vec2NodeViewModel),    typeof(Vec2NodeModel) },
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
    /// Adds the specified <see cref="NodeViewModel"/> to the editor’s view-model
    /// collection and registers it for connection-related events.
    /// </summary>
    /// <param name="vm">
    /// The node view model to add to the visual representation of the graph.
    /// </param>
    public void AddNodeViewModel(NodeViewModel vm)
    {
        SubscribeConnectionEvents(vm);
        Nodes.Add(vm);
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="IntNodeViewModel"/> at a position 
    /// derived from the provided transform, using a fixed offset.
    /// Intended for toolbar or menu actions.
    /// </summary>
    [RelayCommand]
    private Task OnAddIntNode(TransformGroup transform)
    {
        AddNodeAtLocation<IntNodeViewModel>(GetLocation(transform, new Vector(60, 60)));
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Creates and adds a new <see cref="IntNodeViewModel"/> at the current
    /// mouse position within the graph editor.
    /// </summary>
    [RelayCommand]
    private Task OnAddIntNodeFromMouse(TransformGroup transform)
    {
        AddNodeAtLocation<IntNodeViewModel>(GetLocation(transform, LastMousePosition));
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
    private void AddNodeAtLocation<T>(Avalonia.Point location)
        where T : NodeViewModel
    {
        var nodeViewModel = _selectedNodeContainer is IBlueprint 
            ? NewBlueprintNodeViewModel<T>(location: location) 
            : NewNodeViewModel<T>(location: location);
        AddNodeViewModel(nodeViewModel);
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
    
    public bool IsEnabled => _selectedNodeContainer is not null;
    public PendingConnectionViewModel PendingConnection { get; }
    public ObservableCollection<NodeViewModel> Nodes { get; set; } = [];
    public ObservableCollection<ConnectionViewModel> Connections { get; set; } = [];
    public Avalonia.Point LastMousePosition { get; set; } = new Point(0, 0);
    
    public NodeViewModel? SelectedNode
    {
        get => _selectedNode;
        set
        {
            if (value is null)
            {
                return;    
            }
            
            _selectedNode = value;
            _selectionService.SelectedObject = value;
            OnPropertyChanged();
        }
    }

    private NodeEditorModel Model { get; }
    
    private readonly ISelectionService _selectionService;
    private NodeViewModel? _selectedNode = null;
    private IGraphContainer? _selectedNodeContainer = null;
}