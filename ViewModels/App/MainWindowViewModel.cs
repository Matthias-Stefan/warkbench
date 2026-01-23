using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using System.Threading.Tasks;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.ui.core.Projects;


namespace warkbench.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel(
        DockFactory dockFactory,
        IProjectService projectService,
        IProjectSession projectSession,
        ICreateProjectDialog createProjectDialog)
    {
        Layout = dockFactory.CreateLayout();
        dockFactory.InitLayout(Layout);
        
        _projectService = projectService;
        _projectSession = projectSession;
        _createProjectDialog = createProjectDialog;
    }
        
    public IRootDock? Layout
    {
        get => _layout;
        set => SetProperty(ref _layout, value);
    }

    [RelayCommand]
    private async Task OnCreateProject()
    {
        var desktop = Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
        
        var owner = desktop?.MainWindow;
        if (owner is null)
            return;

        var createProjectInfo = await _createProjectDialog.ShowAsync(owner);
        if (createProjectInfo is null)
            return;

        var project = _projectService.CreateProject(createProjectInfo.ProjectName);
        if (!createProjectInfo.OpenAfterCreation)
            return;
        
        await _projectSession.SwitchToAsync(project);
        CurrentProjectName = project.Name;
    }

    [RelayCommand]
    private Task OnOpenProject()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnSaveAll()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnExit()
    {
        return Task.CompletedTask;
    }

    private void CloseProject()
    {
        
    }

    [ObservableProperty] 
    private string _currentProjectName = string.Empty;
    
    private IRootDock? _layout;
    
    private readonly IProjectService _projectService;
    private readonly IProjectSession _projectSession;
    private readonly ICreateProjectDialog _createProjectDialog;
}