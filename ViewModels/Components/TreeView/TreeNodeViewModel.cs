using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace warkbench.ViewModels;

public partial class TreeNodeViewModel : ObservableObject
{
    private readonly INameable _data;
    public object Data => _data;
    public string Name => _data.Name;
    
    [ObservableProperty]
    private TreeNodeViewModel? _parent;

    [ObservableProperty]
    private ObservableCollection<TreeNodeViewModel> _children;

    public void AddChild(TreeNodeViewModel? node)
    {
        if (node is null)
        {
            return;
        }

        Children.Add(node);
    }

    public bool RemoveChild(TreeNodeViewModel? node)
    {
        if (node is null)
        {
            return false;
        }

        return Children.Remove(node) || Children.Any(child => child.RemoveChild(node));
    }

    public void UpdateName(string name)
    {
        _data.Name = name;
    }

    public TreeNodeViewModel(INameable data, IEnumerable<TreeNodeViewModel>? children = null)
    {
        // TODO:
        data.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(INameable.Name))
            {
                OnPropertyChanged(nameof(Name));
            }
        };
            
        _data = data;
        _parent = null;
        _children = [];
        if (children is not null)
        {
            _children = [.. children];
        }
    }
}