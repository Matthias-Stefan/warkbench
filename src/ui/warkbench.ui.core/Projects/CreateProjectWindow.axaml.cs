using Avalonia.Controls;
using warkbench.src.editors.core.ViewModel;

namespace warkbench.src.ui.core.Projects;

public partial class CreateProjectWindow : Window
{
    public CreateProjectWindow()
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