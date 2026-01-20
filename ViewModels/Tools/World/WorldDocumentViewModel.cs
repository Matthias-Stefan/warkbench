using Avalonia.Media.Imaging;
using Avalonia;
using Dock.Model.Mvvm.Controls;
using System.Collections.ObjectModel;
using warkbench.src.ui.viewport.Controls;
using warkbench.src.ui.viewport.Rendering;

namespace warkbench.ViewModels;

public class WorldDocumentViewModel : Document
{
    public WorldDocumentViewModel()
    {
        ActiveTool = ViewportTool.Selection;
            
        Bitmap player = new Bitmap("C:\\Users\\mat019\\Documents\\code\\warpunk.emberfall\\assets\\Sprites\\Player\\Player.png");
        //Bitmap player = new Bitmap("/home/ms/Documents/warpunk.emberfall/assets/Sprites/Player/Player.png");
#if true
        Renderables.Add(new SpriteRenderable(
            bitmap: player, 
            bounds: new Rect(new Point(200, 200), new Size(32, 32)), 
            layer: 0, 
            sourceRect: new Rect(new Point(0, 0), new Size(32, 32))));
#endif
    }

    public ViewportTool ActiveTool
    {
        get => _activeTool;
        set
        {
            _activeTool = value;
            OnPropertyChanged();

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

    private ViewportTool _activeTool = ViewportTool.Selection;
    private bool _isSelectToolEnabled = true;
}