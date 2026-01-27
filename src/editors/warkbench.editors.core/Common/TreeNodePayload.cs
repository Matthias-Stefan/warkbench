using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.src.editors.core.Models;

namespace warkbench.src.editors.core.ViewModel;

public partial class TreeNodePayload(string name, object? data, LoadState? loadState) : ObservableObject
{
    [ObservableProperty] private string _name = name;
    [ObservableProperty] private object? _data = data;
    [ObservableProperty] private LoadState? _loadState = loadState;
}