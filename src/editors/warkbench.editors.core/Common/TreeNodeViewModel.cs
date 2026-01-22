using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

// ReSharper disable once CheckNamespace
namespace warkbench.src.editors.core.ViewModel;

public sealed partial class TreeNodeViewModel : ObservableObject, ITreeNode
{
    public TreeNodeViewModel(string name, object? data)
    {
        Name = name;
        Parent = null;
        Children = new ReadOnlyObservableCollection<ITreeNode>(_children);
        Data = data;
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

    public string Name { get; }
    public ITreeNode? Parent { get; private set; }
    public ReadOnlyObservableCollection<ITreeNode> Children { get; }

    [ObservableProperty] private object? _data = null;
    [ObservableProperty] private bool _isExpanded = false;
    [ObservableProperty] private bool _isSelected = false;
    
    private readonly ObservableCollection<ITreeNode> _children = [];
}