using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Mvvm.Controls;
using warkbench.viewport;


namespace warkbench.ViewModels;
public class WorldDocumentViewModel : Document
{
    public WorldDocumentViewModel()
    {
        ActiveTool = ViewportTool.Selection;
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

    private viewport.ViewportTool _activeTool = ViewportTool.Selection;
    private bool _isSelectToolEnabled = true;
}