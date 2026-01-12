using Avalonia.Controls;
using Avalonia.Interactivity;
using warkbench.ViewModels;


namespace warkbench.Views;
public partial class CreateWorldWindow : Window
{
    public CreateWorldWindow() => InitializeComponent();

    private void OnCancel(object sender, RoutedEventArgs e) => Close(null);

    private void OnCreate(object sender, RoutedEventArgs e)
    {
        Close(new NewWorldSettings
        {
            Name = NameInput.Text,
            TileSize = (int)TileSizeInput.Value,
            ChunkResolution = (int)ChunkResInput.Value
        });
    }
}