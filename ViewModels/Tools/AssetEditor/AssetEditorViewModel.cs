using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using warkbench.Infrastructure;
using warkbench.Models;


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
        Packages = new TreeNodeViewModel(new RootPackageViewModel(rootVirtualPath));
        Blueprints = new TreeNodeViewModel(new RootPackageBlueprintViewModel(rootVirtualPath));
        Properties = new TreeNodeViewModel(new RootPropertiesViewModel(rootVirtualPath));
        
        Assets = [ Packages, Blueprints, Properties ];

        if (_projectManager.CurrentProject is not null)
        {
            OnProjectChanged(this, _projectManager.CurrentProject);
        }

        _projectManager.ProjectChanged += OnProjectChanged;
    }

    private void OnSelectionChanged(object? obj)
    {
        if (obj is not BasePackageViewModel)
        {
            _selectedAsset = null;
            OnPropertyChanged(nameof(SelectedAsset));
        }
    }
    
    private void OnProjectChanged(object? sender, Project project)
    {
        Packages.Children.Clear();
        Blueprints.Children.Clear();

        foreach (var packageBlueprint in project.PackageBlueprints)
        {
            AddBlueprintNode(new BlueprintViewModel(packageBlueprint));
        }
        
        foreach (var package in project.Packages)
        {
            var blueprint = project.PackageBlueprints.FirstOrDefault(blueprint => blueprint.Guid == package.BlueprintGuid);
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
        
                var item = new PackageItemViewModel(packageViewModel.Blueprint);
                packageViewModel.Model.PackageItems.Add(packageItemModel);
                packageNode.AddChild(new TreeNodeViewModel(item));
        
                OnPropertyChanged(nameof(Assets));
            }
        }
    }

    public void AddPackage(PackageViewModel vm)
    {
        if (_projectManager.CurrentProject is null)
        {
            return;
        }

        var graphModel = Blueprints.Children
            .Select(t => t.Data)
            .OfType<BlueprintViewModel>()
            .Where(pbvm => pbvm.Model.Guid == vm.Model.BlueprintGuid)
            .Select(pbvm => pbvm.Model)
            .FirstOrDefault();
        if (graphModel is null)
        {
            return;
        }
        
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
        {
            return;
        }

        _projectManager.CurrentProject.Packages.Remove(vm.Model);
        RemovePackageNode(vm);
    }

    public void RemovePackageNode(PackageViewModel vm)
    {
        var node = Packages.Children.FirstOrDefault(node => node.Data == vm);
        if (node is null)
        {
            return;
        }

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
        {
            return;
        }

        _projectManager.CurrentProject.PackageBlueprints.Add(vm.Model);
        AddBlueprintNode(vm);
    }

    public void AddBlueprintNode(BlueprintViewModel vm)
    {
        Blueprints.AddChild(new TreeNodeViewModel(vm));
        OnPropertyChanged(nameof(Assets));
    }

    [RelayCommand]
    private void OnRemovePackageBlueprint(BlueprintViewModel vm) => RemovePackageBlueprint(vm);

    public void RemovePackageBlueprint(BlueprintViewModel packageBlueprintViewModel)
    {
        if (_projectManager.CurrentProject is null)
        {
            return;
        }

        var node = Blueprints.Children.Where(tree => tree.Data == packageBlueprintViewModel)?.First();
        if (node is null)
        {
            return;
        }

        _projectManager.CurrentProject.PackageBlueprints.Remove(packageBlueprintViewModel.Model);
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
        
        var item = new PackageItemViewModel(packageViewModel.Blueprint);
        packageViewModel.Model.PackageItems.Add(item.Model);
        packageNode.AddChild(new TreeNodeViewModel(item));
        
        OnPropertyChanged(nameof(Assets));
    }

    public void AddProperty()
    {
        
    }

    private AssetEditorModel Model { get; }

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
