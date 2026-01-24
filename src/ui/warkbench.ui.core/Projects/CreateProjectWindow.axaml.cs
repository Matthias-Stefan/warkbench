using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using warkbench.src.editors.core.Projects;

namespace warkbench.src.ui.core.Projects;

public partial class CreateProjectWindow : Window
{
    public CreateProjectWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }
    
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // Run after layout so focus/caret is guaranteed.
        Dispatcher.UIThread.Post(() =>
        {
            if (TextBox_ProjectName is null)
                return;

            TextBox_ProjectName.Focus();
        }, DispatcherPriority.Loaded);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is CreateProjectViewModel vm)
            vm.RequestClose += Close;
    }
}