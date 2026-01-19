using warkbench.Models;


namespace warkbench.ViewModels;

public partial class PngFileViewModel : FileViewModel
{
    public PngFileViewModel(FileModel model) : base(model)
    {
        ItemType = FileSystemItemType.PngFile;
    }
}