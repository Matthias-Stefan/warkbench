using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.editors.core.Models;

// ReSharper disable once CheckNamespace
namespace warkbench.src.editors.core.ViewModel;

public sealed partial class TreeNodeViewModel : ObservableObject, ITreeNode, IDisposable
{
    public TreeNodeViewModel(string name, object? data, LoadState? loadState = null)
    {
        Name = name;
        Parent = null;
        Children = new ReadOnlyObservableCollection<ITreeNode>(_children);
        Data = data;

        if (Data is IDirtyable dirtyable)
            dirtyable.IsDirtyChanged += OnDirtyChanged!;

        LoadState = loadState;
    }
    
    public void Dispose()
    {
        if (Data is IDirtyable dirtyable)
            dirtyable.IsDirtyChanged -= OnDirtyChanged!;
    }

    public void AddChild(ITreeNode node)
    {
        if (node is TreeNodeViewModel vm)
        {
            vm.Parent = this;
        }
        
        _children.Add(node);
    }

    public bool RemoveChild(ITreeNode node)
    {
        return _children.Remove(node);
    }

    public void RemoveFromParent()
    {
        Parent?.RemoveChild(this);
    }

    public void ExpandAll()
    {
        IsExpanded = true;
        foreach (var child in Children)
        {
            child.ExpandAll();
        }
    }

    public void CollapseAll()
    {
        IsExpanded = false; 
        foreach (var child in Children)
        {
            child.CollapseAll();
        }
    }

    public string GetFullPath(string separator = "/")
    {
        return Parent == null ? Name : $"{Parent.GetFullPath(separator)}{separator}{Name}";
    }

    private void OnDirtyChanged(object sender, EventArgs e)
    {
        if (sender is not IDirtyable dirtyable)
            return;

        if (IsDirty == dirtyable.IsDirty)
            return;
        
        IsDirty = dirtyable.IsDirty;
        OnPropertyChanged(nameof(DisplayName));
    }
    
    public string Name
    {
        get => _name;
        private init => SetProperty(ref _name, value);
    }

    public string DisplayName => IsDirty ? $"{Name}*" : Name;

    public bool IsDirty
    {
        get => _isDirty;
        private set => SetProperty(ref _isDirty, value);
    }

    [ObservableProperty]
    private LoadState? _loadState;
    public string LoadStateText => LoadState?.ToString() ?? string.Empty;
    
    
    public ITreeNode? Parent { get; private set; }
    public ReadOnlyObservableCollection<ITreeNode> Children { get; }

    [ObservableProperty] private object? _data = null;
    [ObservableProperty] private bool _isExpanded = false;
    [ObservableProperty] private bool _isSelected = false;
    
    private readonly ObservableCollection<ITreeNode> _children = [];
    private readonly string _name;
    private bool _isDirty;
}