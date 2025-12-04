using warkbench.Models;


namespace warkbench.ViewModels;
public partial class CppFileViewModel : FileViewModel
{
    public CppFileViewModel(FileModel model) : base(model)
    {
        ItemType = FileSystemItemType.CppFile;
    }
}