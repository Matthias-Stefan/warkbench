using CommunityToolkit.Mvvm.ComponentModel;

namespace warkbench.ViewModels;

public partial class BreadcrumbViewModel : ObservableObject
{
    public FolderViewModel Folder { get; }

    public BreadcrumbViewModel(FolderViewModel folder)
    {
        Folder = folder;
    }
}