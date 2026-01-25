using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
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
        try
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
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }
}