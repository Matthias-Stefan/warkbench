using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.editors.core.Models;

namespace warkbench.src.editors.core.ViewModel;

public sealed partial class TreeNodeViewModel : ObservableObject, ITreeNode, IDisposable
{
    public TreeNodeViewModel(TreeNodePayload payload)
    {
        _payload = payload;
        OnPropertyChanged(nameof(Data));
        
        Children = new ReadOnlyObservableCollection<ITreeNode>(_children);
        Parent = null;
        
        if (_payload.Data is IDirtyable dirtyable)
            dirtyable.IsDirtyChanged += OnDirtyChanged;
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
    
    public string Name => _payload.Name;

    public object? Data
    {
        get => _payload.Data;
        set
        {
            if (ReferenceEquals(_payload.Data, value))
                return;

            // unsubscribe old
            if (_payload.Data is IDirtyable oldDirty)
                oldDirty.IsDirtyChanged -= OnDirtyChanged;
            
            _payload.Data = value;

            // subscribe new + refresh
            if (value is IDirtyable newDirty)
            {
                newDirty.IsDirtyChanged += OnDirtyChanged;
                IsDirty = newDirty.IsDirty;
            }
            else
            {
                IsDirty = false;
            }

            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(Data));
        }
    }

    public LoadState? LoadState
    {
        get => _payload.LoadState;
        set
        {
            _payload.LoadState = value;
            OnPropertyChanged(nameof(LoadStateText));
        }
    }
    
    public string LoadStateText
    {
        get
        {
            return LoadState switch
            {
                Models.LoadState.NotLoaded => "Not Loaded",
                Models.LoadState.Loading => "Loading",
                Models.LoadState.Loaded => "Loaded",
                Models.LoadState.Failed => "Failed",
                _ => string.Empty
            };
        }
    }
    
    public string DisplayName => IsDirty ? $"{Name}*" : Name;

    public bool IsDirty
    {
        get => _isDirty;
        private set => SetProperty(ref _isDirty, value);
    }
    
    public ITreeNode? Parent { get; private set; }
    public ReadOnlyObservableCollection<ITreeNode> Children { get; }

    [ObservableProperty] private bool _isExpanded = false;
    [ObservableProperty] private bool _isSelected = false;
    
    private bool _isDirty;
    
    private readonly ObservableCollection<ITreeNode> _children = [];
    private readonly TreeNodePayload _payload;
}