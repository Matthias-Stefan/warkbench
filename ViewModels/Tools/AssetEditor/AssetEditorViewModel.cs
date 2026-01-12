using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Threading.Tasks;
using warkbench.Brushes;
using warkbench.Infrastructure;
using warkbench.Models;
using warkbench.core;


namespace warkbench.ViewModels;
public partial class AssetEditorViewModel : Tool
{
    public AssetEditorViewModel(
        IProjectManager projectManager,
        AssetEditorModel model,
        ISelectionService selectionService)
    {
        Model = model;
        _projectManager = projectManager;
        _selectionService = selectionService;
        selectionService.WhenSelectionChanged.Subscribe(OnSelectionChanged);
        
        var rootVirtualPath = string.Empty;
        Worlds = new TreeNodeViewModel(new RootWorldViewModel(rootVirtualPath));
        Packages = new TreeNodeViewModel(new RootPackageViewModel(rootVirtualPath));
        Blueprints = new TreeNodeViewModel(new RootPackageBlueprintViewModel(rootVirtualPath));
        Properties = new TreeNodeViewModel(new RootPropertiesViewModel(rootVirtualPath));
        
        Assets = [ Worlds, Packages, Blueprints, Properties ];

        if (_projectManager.CurrentProject is not null)
        {
            OnProjectChanged(this, _projectManager.CurrentProject);
        }

        _projectManager.ProjectChanged += OnProjectChanged;
    }

    private void OnSelectionChanged(object? obj)
    {
        if (obj is not AssetViewModel)
        {
            _selectedAsset = null;
            OnPropertyChanged(nameof(SelectedAsset));
        }
    }
    
    private void OnProjectChanged(object? sender, Project project)
    {
        Worlds.Children.Clear();
        Packages.Children.Clear();
        Blueprints.Children.Clear();

        foreach (var world in project.Worlds)
        {
            AddWorldNode(new WorldViewModel(world));
        }

        foreach (var packageBlueprint in project.Blueprints)
        {
            AddBlueprintNode(new BlueprintViewModel(packageBlueprint));
        }
        
        foreach (var package in project.Packages)
        { 
            var blueprint = project.Blueprints.FirstOrDefault(blueprint => blueprint.Guid == package.BlueprintGuid);
            if (blueprint is null)
            {
                continue;
            }

            var packageViewModel = new PackageViewModel(package, blueprint);
            AddPackageNode(packageViewModel);
            
            foreach (var packageItemModel in packageViewModel.Model.PackageItems)
            {
                
                var packageNode = Packages.Children.FirstOrDefault(node => node.Data == packageViewModel);
                if (packageNode is null)
                {
                    return;
                }

                var item = new PackageItemViewModel(packageItemModel, packageViewModel.Blueprint);
                packageNode.AddChild(new TreeNodeViewModel(item));
                OnPropertyChanged(nameof(Assets));
            }
        }

        foreach (var property in project.Properties)
        {
            var propertyViewModel = new PropertyViewModel(property);
            AddPropertyNode(propertyViewModel);
        }
    }

    [RelayCommand]
    public async Task AddWorld()
    {
        var desktop = Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
        
        var owner = desktop?.MainWindow;
        if (owner is null)
            return;
        
        var dialog = new warkbench.Views.CreateWorldWindow();

        var result = await dialog.ShowDialog<NewWorldSettings>(owner);
        if (result is null)
            return;
        
        var model = new World(result.TileSize, result.ChunkResolution)
        {
            Guid = Guid.NewGuid(),
            Name = result.Name
        };
    
        AddWorld(model);
    }

    public void AddWorld(World model)
    {
        AddWorld(new WorldViewModel(model));
    }

    public void AddWorld(WorldViewModel vm)
    {
        if (_projectManager.CurrentProject is null)
            return;
        
        _projectManager.CurrentProject.Worlds.Add(vm.Model);
        AddWorldNode(vm);
    }

    public void AddWorldNode(WorldViewModel vm)
    {
        Worlds.AddChild(new TreeNodeViewModel(vm));
        OnPropertyChanged(nameof(Assets));
    }

    public void AddPackage(PackageViewModel vm)
    {
        if (_projectManager.CurrentProject is null)
            return;

        var graphModel = Blueprints.Children
            .Select(t => t.Data)
            .OfType<BlueprintViewModel>()
            .Where(pbvm => pbvm.Model.Guid == vm.Model.BlueprintGuid)
            .Select(pbvm => pbvm.Model)
            .FirstOrDefault();

        if (graphModel is null)
            return;
        
        _projectManager.CurrentProject.Packages.Add(vm.Model);
        AddPackageNode(vm);
    }

    public void AddPackageNode(PackageViewModel vm)
    {
        Packages.AddChild(new TreeNodeViewModel(vm));
        OnPropertyChanged(nameof(Assets));
    }

    [RelayCommand]
    private void OnRemovePackage(PackageViewModel vm) => RemovePackage(vm);

    public void RemovePackage(PackageViewModel vm)
    {
        if (_projectManager.CurrentProject is null)
            return;

        _projectManager.CurrentProject.Packages.Remove(vm.Model);
        RemovePackageNode(vm);
    }

    public void RemovePackageNode(PackageViewModel vm)
    {
        var node = Packages.Children.FirstOrDefault(node => node.Data == vm);
        if (node is null)
            return;

        Packages.RemoveChild(node);
        OnPropertyChanged(nameof(Assets));
    }

    public void AddBlueprint()
    {
        AddBlueprint(new GraphModel
            {
                Guid = System.Guid.NewGuid(),
                Name = string.Empty
            }
        );
    }

    public void AddBlueprint(GraphModel model)
    {
        AddBlueprint(new BlueprintViewModel(model));
    }

    public void AddBlueprint(BlueprintViewModel vm)
    {
        if (_projectManager.CurrentProject is null)
            return;

        _projectManager.CurrentProject.Blueprints.Add(vm.Model);
        AddBlueprintNode(vm);
    }

    public void AddBlueprintNode(BlueprintViewModel vm)
    {
        Blueprints.AddChild(new TreeNodeViewModel(vm));
        OnPropertyChanged(nameof(Assets));
    }

    [RelayCommand]
    private void OnRemoveBlueprint(BlueprintViewModel vm) => RemoveBlueprint(vm);

    public void RemoveBlueprint(BlueprintViewModel packageBlueprintViewModel)
    {
        if (_projectManager.CurrentProject is null)
            return;

        var node = Blueprints.Children.Where(tree => tree.Data == packageBlueprintViewModel)?.First();
        if (node is null)
            return;

        _projectManager.CurrentProject.Blueprints.Remove(packageBlueprintViewModel.Model);
        packageBlueprintViewModel.Nodes.Clear();
        packageBlueprintViewModel.Connections.Clear();
        Blueprints.RemoveChild(node);
        OnPropertyChanged(nameof(Assets));
    }

    [RelayCommand]
    private void OnAddPackageItem(PackageViewModel packageViewModel)
    {
        if (SelectedAsset is null)
        {
            return;
        }
        
        AddPackageItemNode(packageViewModel);
    }

    private void AddPackageItemNode(PackageViewModel packageViewModel)
    {
        var packageNode = Packages.Children.FirstOrDefault(node => node.Data == packageViewModel);
        if (packageNode is null)
        {
            return;
        }
        
        var packageItemGraph = packageViewModel.Blueprint.DeepClone();
        foreach (var nodeModel in packageItemGraph.Nodes)
        {
            nodeModel.NodeHeaderBrushType = NodeHeaderBrushType.None;
            if (nodeModel.InternalGraph is not null)
            {
                foreach (var internalGraphNode in nodeModel.InternalGraph.Nodes)
                {
                    internalGraphNode.NodeHeaderBrushType = NodeHeaderBrushType.None;
                }
            }
        }
        var item = new PackageItemViewModel(packageItemGraph, packageViewModel.Blueprint);
        packageViewModel.Model.PackageItems.Add(item.Model);
        packageNode.AddChild(new TreeNodeViewModel(item));
        
        OnPropertyChanged(nameof(Assets));
    }

    public void AddProperty()
    {
        AddProperty(new GraphModel
            {
                Guid = System.Guid.NewGuid(),
                Name = string.Empty
            }
        );
    }

    public void AddProperty(GraphModel model)
    {
        AddProperty(new PropertyViewModel(model));
    }

    public void AddProperty(PropertyViewModel vm)
    {
        if (_projectManager.CurrentProject is null)
        {
            return;
        }

        _projectManager.CurrentProject.Properties.Add(vm.Model);
        AddPropertyNode(vm);
    }

    public void AddPropertyNode(PropertyViewModel vm)
    {
        Properties.AddChild(new TreeNodeViewModel(vm));
        OnPropertyChanged(nameof(Assets));
    }

    [RelayCommand]
    private void OnRemoveProperty(PropertyViewModel vm) => RemoveProperty(vm);

    public void RemoveProperty(PropertyViewModel propertyViewModel)
    {
        if (_projectManager.CurrentProject is null)
        {
            return;
        }

        var node = Properties.Children.Where(tree => tree.Data == propertyViewModel)?.First();
        if (node is null)
        {
            return;
        }

        _projectManager.CurrentProject.Properties.Remove(propertyViewModel.Model);
        propertyViewModel.Nodes.Clear();
        propertyViewModel.Connections.Clear();
        Properties.RemoveChild(node);
        OnPropertyChanged(nameof(Assets));
    }

    private AssetEditorModel Model { get; }

    public TreeNodeViewModel Worlds { get; }
    public ObservableCollection<TreeNodeViewModel> Assets { get; }
    public TreeNodeViewModel Packages { get; }
    public TreeNodeViewModel Blueprints { get; }
    public TreeNodeViewModel Properties { get;  }

    public TreeNodeViewModel? SelectedAsset
    {
        get => _selectedAsset;
        set
        {
            if (value is null)
            {
                return;
            }

            _selectedAsset = value;
            _selectionService.SelectedObject = value.Data;
            OnPropertyChanged();
        }
    }

    private readonly IProjectManager _projectManager;
    private readonly ISelectionService _selectionService;
    private TreeNodeViewModel? _selectedAsset;
}
