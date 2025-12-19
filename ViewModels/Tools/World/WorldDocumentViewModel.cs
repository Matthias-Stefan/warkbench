using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using warkbench.viewport;


namespace warkbench.ViewModels;
public class WorldDocumentViewModel : Document
{



    public viewport.ViewportTool ActiveTool
    {
        get => _activeTool;
        set
        {
            OnPropertyChanging(nameof(ActiveTool));
            _activeTool = value;
            OnPropertyChanged(nameof(ActiveTool));
        }
    }

    public bool IsSelectToolEnabled
    {
        get => ActiveTool == ViewportTool.Selection;
        set => ActiveTool = value ? ViewportTool.Selection : ViewportTool.None;
    }
    
    private viewport.ViewportTool _activeTool = ViewportTool.Selection;
}