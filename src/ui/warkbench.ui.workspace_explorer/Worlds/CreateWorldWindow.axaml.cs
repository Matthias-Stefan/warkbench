using Avalonia.Controls;
using Avalonia.Threading;
using warkbench.src.editors.core.Projects;
using warkbench.src.editors.core.Worlds;

namespace warkbench.src.ui.workspace_explorer.Worlds;

public partial class CreateWorldWindow : Window, IDisposable
{
    public CreateWorldWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }
    
    public void Dispose()
    {
        if (DataContext is CreateWorldViewModel vm)
            vm.RequestClose -= Close;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // Run after layout so focus/caret is guaranteed.
        Dispatcher.UIThread.Post(() =>
        {
            if (TextBox_WorldName is null)
                return;

            TextBox_WorldName.Focus();
        }, DispatcherPriority.Loaded);
    }
    
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is CreateWorldViewModel vm)
            vm.RequestClose += Close;
    }
}