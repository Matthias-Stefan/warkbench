using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using warkbench.src.editors.workspace_explorer.ViewModels;
using warkbench.src.ui.workspace_explorer.Worlds;

namespace warkbench.src.ui.workspace_explorer.Views;

public partial class WorkspaceExplorerView : UserControl
{
    public WorkspaceExplorerView()
    {
        InitializeComponent();
    }

    private async void OnCreateNewWorld(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (DataContext is not WorkspaceExplorerViewModel vm)
            return;
    
        var desktop = Avalonia.Application.Current?.ApplicationLifetime as 
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
    
        var window = desktop?.MainWindow;
        if (window is null)
            return;
    
        ICreateWorldDialog createWorldDialog = new CreateWorldDialog();
        if (createWorldDialog == null) 
            throw new ArgumentNullException(nameof(createWorldDialog));
    
        var createWorldInfo = await createWorldDialog.ShowAsync(window);
        if (createWorldInfo is null)
            return;

        await vm.OnCreateNewWorld(createWorldInfo);
    }

    private void TextBox_DisplayName_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is not TextBox tb) 
            return;

        if (tb.IsKeyboardFocusWithin) 
            return;
        
        #if false
        if (DataContext is WorkspaceExplorerViewModel vm)
            vm.CommitRenameCommand.Execute(null);
#endif 
    }

    private void TextBox_DisplayName_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not WorkspaceExplorerViewModel vm)
            return;

        switch (e.Key)
        {
            case Key.Enter: 
                vm.CommitRenameCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.Escape:
                vm.CancelRenameCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.Left:
            case Key.Up:
            case Key.Down:
            case Key.Right:
                e.Handled = true;
                break;
        }
    }
    
    private void TextBox_DisplayName_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        if (e.Property != Visual.IsVisibleProperty ||
            e.NewValue is not true) 
            return;
        
        textBox.Focus();

        var text = textBox.Text ?? string.Empty;

        var dotIndex = text.LastIndexOf('.');

        if (dotIndex > 0)
        {
            textBox.SelectionStart = 0;
            textBox.SelectionEnd   = dotIndex;
        }
        else
        {
            textBox.SelectAll();
        }
    }
}