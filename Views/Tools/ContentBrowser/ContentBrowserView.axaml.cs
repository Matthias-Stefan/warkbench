using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using warkbench.Models;
using warkbench.ViewModels;

namespace warkbench.Views;

public partial class ContentBrowserView : UserControl
{
    public ContentBrowserView()
    {
        InitializeComponent();
    }

    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is not null && sender is Border border && border.DataContext is FileSystemItemViewModel)
        {
            border.Background = new SolidColorBrush(new Color(255, 87, 89, 94)); ;
        }
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is not null && sender is Border border && border.DataContext is FileSystemItemViewModel)
        {
            border.Background = new SolidColorBrush(new Color(255, 57, 59, 64)); ;
        }
    }

    private async void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Visual);
        if (point.Properties.IsLeftButtonPressed && e.ClickCount == 1)
        {
            if (sender is Border border)
            {
                // Files
                if (border.DataContext is FileViewModel file)
                {
                    var data = new DataObject();
                    data.Set(typeof(FileViewModel).FullName, file);
                    await DragDrop.DoDragDrop(e, data, DragDropEffects.Copy);
                    return;
                }

                // Folder
                if (border.DataContext is FolderViewModel folder)
                {
                    if (DataContext is ContentBrowserViewModel contentBrowser)
                    {
                        contentBrowser.Open(folder);
                    }
                }
            }


        }

        if (point.Properties.IsRightButtonPressed)
        {
        }
    }

    private async void OnOpenFolder(object? sender, RoutedEventArgs e)
    {
        Window window = (Window)this.VisualRoot!;

        var folder = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select folder",
            AllowMultiple = false
        });
        
        if (folder.Count > 0)
        {
            var selectedFolder = folder[0];
            if (DataContext is ContentBrowserViewModel contentBrowser)
            {
                contentBrowser.AddToHome(new FolderViewModel(new FolderModel(selectedFolder.Path.AbsolutePath)));
            }
        }
    }
}