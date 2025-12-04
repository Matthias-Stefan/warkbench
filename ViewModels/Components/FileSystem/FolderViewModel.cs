using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using warkbench.Models;

namespace warkbench.ViewModels;

public partial class FolderViewModel : FileSystemItemViewModel
{
    public FolderModel Model { get; }
    public FolderViewModel? Parent { get; set; }
    public ObservableCollection<FolderViewModel> SubFolders { get; }
    public ObservableCollection<FileViewModel> Files { get; }

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isSelectable = true;

    public FolderViewModel(FolderModel model)
    {
        Model = model;

        Name = model.Name;
        FullPath = model.FullPath;
        LastModified = model.LastModified;
        ItemType = FileSystemItemType.Folder;

        SubFolders = new ObservableCollection<FolderViewModel>();
        foreach (var subFolderModel in model.SubFolders)
        {
            var vm = new FolderViewModel(subFolderModel) { Parent = this };
            SubFolders.Add(vm);
        }

        Files = new ObservableCollection<FileViewModel>();
        foreach (var fileModel in model.Files)
        {
            Files.Add(new FileViewModel(fileModel));
        }
    }
}