using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.editors.core.Models;

namespace warkbench.src.editors.core.ViewModel;

public sealed partial class TreeNodeViewModel : ObservableObject, IDisposable
{
    public TreeNodeViewModel(TreeNodePayload payload)
    {
        _payload = payload;
        OnPropertyChanged(nameof(Data));
        
        Children = new ReadOnlyObservableCollection<TreeNodeViewModel>(_children);
        Parent = null;
        
        if (_payload.Data is IDirtyable dirtyable)
            dirtyable.IsDirtyChanged += OnDirtyChanged!;

        if (_payload.Data is IRenameable renameable)
            renameable.NameChanged += OnNameChanged!;
    }
    
    public void Dispose()
    {
        if (Data is IDirtyable dirtyable)
            dirtyable.IsDirtyChanged -= OnDirtyChanged!;
        if (Data is IRenameable renameable)
            renameable.NameChanged -= OnNameChanged!;
    }

    public void AddChild(TreeNodeViewModel node)
    {
        if (node is TreeNodeViewModel vm)
        {
            vm.Parent = this;
        }
        
        _children.Add(node);
    }

    public bool RemoveChild(TreeNodeViewModel node)
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

    public void BeginRename()
    {
        if (_payload.Data is not IRenameable renameable)
            return;
        
        IsRenaming = true;
        EditName = Name;
    }
    public void CancelRename() => IsRenaming = false;

    private void OnDirtyChanged(object sender, EventArgs e)
    {
        if (sender is not IDirtyable dirtyable)
            return;

        if (IsDirty == dirtyable.IsDirty)
            return;
        
        IsDirty = dirtyable.IsDirty;
        OnPropertyChanged(nameof(Name));
    }

    private void OnNameChanged(object sender, EventArgs e)
    {
        if (sender is not IRenameable renameable)
            return;

        if (Name == renameable.Name)
            return;

        _payload.Name = renameable.Name;
        
        IsRenaming = false;
        IsDirty = true;
        OnPropertyChanged(nameof(Name));
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
                oldDirty.IsDirtyChanged -= OnDirtyChanged!;
            if (_payload.Data is IRenameable renameable)
                renameable.NameChanged -= OnNameChanged!;
            
            _payload.Data = value;

            // subscribe new + refresh
            if (value is IDirtyable newDirty)
                newDirty.IsDirtyChanged += OnDirtyChanged!;
            if (value is  IRenameable newRenameable)
                newRenameable.NameChanged += OnNameChanged!;
            
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged();
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
    
    public TreeNodeViewModel? Parent { get; private set; }
    public ReadOnlyObservableCollection<TreeNodeViewModel> Children { get; }
    
    [ObservableProperty] private bool _isRenaming = false;
    [ObservableProperty] private bool _isExpanded = false;
    [ObservableProperty] private bool _isSelected = false;
    [ObservableProperty] private bool _isDirty = false;
    [ObservableProperty] private string _editName = string.Empty;
    
    private readonly ObservableCollection<TreeNodeViewModel> _children = [];
    private readonly TreeNodePayload _payload;
}