using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using System.Threading.Tasks;
using System;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Selection;
using warkbench.src.ui.core.Projects;

namespace warkbench.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    public MainWindowViewModel(
        DockFactory dockFactory,
        ICreateProjectDialog createProjectDialog,
        ILogger logger,
        IPathService pathService,
        IProjectService projectService,
        IProjectSession projectSession,
        ISelectionCoordinator selectionCoordinator,
        IProjectPicker projectPicker
        )
    {
        Layout = dockFactory.CreateLayout();
        dockFactory.InitLayout(Layout);
        
        _createProjectDialog = createProjectDialog;
        _logger = logger;
        _pathService = pathService;
        _projectService = projectService;
        _projectSession = projectSession;
        _selectionCoordinator = selectionCoordinator;
        _projectPicker = projectPicker;
        
        _selectionSubscription = selectionCoordinator.Subscribe(SelectionScope.Project);
        _selectionSubscription.Changed += OnSelectionChanged;
    }
        
    public void Dispose()
    {
        _selectionSubscription.Changed -= OnSelectionChanged;
        _selectionSubscription.Dispose();
    }
    
    public IRootDock? Layout
    {
        get => _layout;
        set => SetProperty(ref _layout, value);
    }

    [RelayCommand]
    private async Task OnCreateProject()
    {
        var createProjectInfo = await _createProjectDialog.ShowAsync();
        if (createProjectInfo is null)
            return;

        var project = await _projectService.CreateProjectAsync(createProjectInfo.ProjectName);
        if (!createProjectInfo.OpenAfterCreation)
            return;
        
        await _projectSession.SwitchToAsync(project);
        UpdateWindowBarProjectTitle(project);
        _logger.Info<MainWindowViewModel>("Project created successfully.");
    }

    [RelayCommand]
    private async Task OnOpenProject()
    {
        var path = await _projectPicker.Open();
        if (string.IsNullOrEmpty(path))
            return;
        
        var currentProject = _selectionCoordinator.CurrentProject;
        if (currentProject is not null && path.Contains(currentProject.LocalPath.Value))
        {
            _logger.Warn<MainWindowViewModel>("The selected project is already open and will not be reloaded.");
            return;
        }
        
        var absoluteProjectPath = new AbsolutePath(path);
        var project = await _projectService.LoadProjectAsync(absoluteProjectPath);
        
        await _projectSession.SwitchToAsync(project);
        UpdateWindowBarProjectTitle(project);
        _logger.Info<MainWindowViewModel>("Project opened successfully.");
    }

    [RelayCommand]
    private Task OnSaveAll()
    {
        // TODO:
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnExit()
    {
        // TODO:
        return Task.CompletedTask;
    }

    private void UpdateWindowBarProjectTitle(IProject? project)
    {
        WindowBarProjectTitle = project?.Name ?? string.Empty;
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs<object> e)
        => UpdateWindowBarProjectTitle(_selectionCoordinator.CurrentProject);

    [ObservableProperty] 
    private string _windowBarProjectTitle = string.Empty;
    
    private IRootDock? _layout;
    
    private readonly ISelectionSubscription _selectionSubscription;
    
    private readonly ICreateProjectDialog _createProjectDialog;
    private readonly ILogger _logger;
    private readonly IPathService _pathService;
    private readonly IProjectService _projectService;
    private readonly IProjectSession _projectSession;
    private readonly ISelectionCoordinator _selectionCoordinator;
    private readonly IProjectPicker _projectPicker;
}