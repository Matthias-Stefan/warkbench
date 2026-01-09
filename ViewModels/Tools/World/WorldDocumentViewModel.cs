using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using warkbench.viewport;


namespace warkbench.ViewModels;
public class WorldDocumentViewModel : Document
{
    public WorldDocumentViewModel()
    {
        ActiveTool = ViewportTool.Selection;

        Bitmap player =
            new Bitmap("C:\\Users\\mat019\\Documents\\code\\warpunk.emberfall\\assets\\Sprites\\Player\\Player.png");
        Renderables.Add(new SpriteRenderable(player, new Rect(new Point(0, 0), player.Size), 0, new Rect(new Point(0, 0), new Size(32, 32))));
    }

    public viewport.ViewportTool ActiveTool
    {
        get => _activeTool;
        set
        {
            OnPropertyChanging(nameof(ActiveTool));
            _activeTool = value;
            OnPropertyChanged(nameof(ActiveTool));

            if (ActiveTool == ViewportTool.Selection)
            {
                IsSelectToolEnabled = true;
            }
        }
    }

    public bool IsSelectToolEnabled
    {
        get => _isSelectToolEnabled;
        set
        {
            if (value == _isSelectToolEnabled)
            {
                return;    
            }
            
            _isSelectToolEnabled = value;
            ActiveTool = _isSelectToolEnabled ? ViewportTool.Selection : ViewportTool.None;
        }
    }

    public ObservableCollection<IRenderable> Renderables { get; set; } = [];    

    private viewport.ViewportTool _activeTool = ViewportTool.Selection;
    private bool _isSelectToolEnabled = true;
}