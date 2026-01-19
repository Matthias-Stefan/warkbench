using warkbench.Models;


namespace warkbench.ViewModels;

public partial class HppFileViewModel : FileViewModel
{
    public HppFileViewModel(FileModel model) : base(model)
    {
        ItemType = FileSystemItemType.HppFile;
    }
}