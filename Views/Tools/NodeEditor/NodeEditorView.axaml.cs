using Avalonia.Controls;
using Avalonia.Input;
using warkbench.ViewModels;


namespace warkbench.Views;
public partial class NodeEditorView : UserControl
{
    public NodeEditorView()
    {
        InitializeComponent();
    }

    private void NodifyEditor_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab)
        {
            if (Editor.ContextMenu is { } menu)
            {
                menu.PlacementTarget = Editor;

                menu.Open();
            }
        }
        else if (e.Key == Key.Delete)
        {
            if (DataContext is NodeEditorViewModel vm)
            {
                vm.RemoveNodeViewModel(Editor.SelectedItem as NodeViewModel);
            }
        }

        e.Handled = true;
    }

    private void NodifyEditor_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is NodeEditorViewModel vm)
        {
            vm.LastMousePosition = e.GetPosition(this);
        }
    }
}