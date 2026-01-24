using Avalonia.Controls;
using warkbench.src.editors.core.Projects;

namespace warkbench.src.ui.core.Projects;

public partial class CreateWroldInfo : Window
{
    public CreateWroldInfo()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is CreateProjectViewModel vm)
            vm.RequestClose += Close;
    }
}