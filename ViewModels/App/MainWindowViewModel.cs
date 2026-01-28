using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using System.Threading.Tasks;
using System;
using warkbench.src.basis.interfaces.Logger;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Selection;

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
        
        await _projectSession.ActivateAsync(project);
        UpdateWindowBarProjectTitle(project);
    }

    [RelayCommand]
    private async Task OnOpenProject()
    {
        var path = await _projectPicker.Open();
        if (string.IsNullOrWhiteSpace(path))
            return;
        
        var currentProject = _selectionCoordinator.CurrentProject;
        if (currentProject is not null && path.Contains(currentProject.LocalPath.Value))
        {
            _logger.Warn<MainWindowViewModel>("The selected project is already open and will not be reloaded.");
            return;
        }
        
        var project = await _projectSession.OpenAsync(new AbsolutePath(path), ProjectLoadMode.ManifestOnly);
        UpdateWindowBarProjectTitle(project);
    }

    [RelayCommand]
    private async Task OnSaveAll()
    {
        var currentProject = _selectionCoordinator.CurrentProject;
        if (currentProject is null)
        {
            _logger.Warn<MainWindowViewModel>(
                "Save operation requested, but no project is currently open.");
            return;
        }

        _logger.Info<MainWindowViewModel>(
            "Saving project and all associated data.");

        // TODO: cascading save (project + worlds + assets)
        await _projectService.SaveProjectAsync(currentProject);

        _logger.Info<MainWindowViewModel>(
            "Project and all changes saved successfully.");
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