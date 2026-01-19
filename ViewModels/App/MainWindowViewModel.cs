using System.Globalization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using warkbench.Infrastructure;
using warkbench.Models;


namespace warkbench.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel(DockFactory dockFactory, IOService exportService, IProjectManager projectManager)
    {
        Layout = dockFactory.CreateLayout();
        dockFactory.InitLayout(Layout);
        ExportService = exportService;
        ProjectManager = projectManager;
    }
        
    public IRootDock? Layout
    {
        get => _layout;
        set => SetProperty(ref _layout, value);
    }

    public string CurrentProjectName => ProjectManager?.CurrentProject is null ? string.Empty : ProjectManager.CurrentProject.Name;

    [RelayCommand]
    private Task OnSaveProject()
    {
        ProjectManager?.Save();
        return Task.CompletedTask;
    }

    private IRootDock? _layout;
        
    private Project Project { get; set; }
    private IOService ExportService { get; }
    private IProjectManager? ProjectManager { get; }
}