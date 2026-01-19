using System;
using warkbench.Models;
using warkbench.ViewModels;

namespace warkbench.ViewModels;

public partial class FileViewModel : FileSystemItemViewModel
{
    public FileModel Model { get; }
    public FolderViewModel Parent { get; set; }

    public FileViewModel(FileModel model)
    {
        Model = model;

        Name = model.Name;
        FullPath = model.FullPath;
        LastModified = model.LastModified;
        ItemType = FileSystemItemType.File;
    }
}