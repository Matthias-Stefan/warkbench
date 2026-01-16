using Avalonia.Media.Imaging;
using Avalonia;
using Dock.Model.Mvvm.Controls;
using System.Collections.ObjectModel;
using warkbench.viewport;


namespace warkbench.ViewModels;
public class WorldDocumentViewModel : Document
{
    public WorldDocumentViewModel()
    {
        ActiveTool = ViewportTool.Selection;
            
        //Bitmap player = new Bitmap("C:\\Users\\mat019\\Documents\\code\\warpunk.emberfall\\assets\\Sprites\\Player\\Player.png");
        Bitmap player = new Bitmap("/home/ms/Documents/warpunk.emberfall/assets/Sprites/Player/Player.png");
        Renderables.Add(new SpriteRenderable(
            bitmap: player, 
            bounds: new Rect(new Point(200, 200), new Size(32, 32)), 
            layer: 0, 
            sourceRect: new Rect(new Point(0, 0), new Size(32, 32))));
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