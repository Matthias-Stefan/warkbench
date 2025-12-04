using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace warkbench.ViewModels;

public partial class FileSystemItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _fullPath = string.Empty;

    [ObservableProperty]
    private DateTime _lastModified = DateTime.MinValue;

    [ObservableProperty]
    private FileSystemItemType _itemType;
}