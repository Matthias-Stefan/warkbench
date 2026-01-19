using CommunityToolkit.Mvvm.ComponentModel;
using System;
using warkbench.Infrastructure;
using warkbench.Models;


namespace warkbench.ViewModels;

public interface IProjectManager
{
    void Save();
    bool Load(string? path);
    
    event EventHandler<Project>? ProjectChanging;
    event EventHandler<Project>? ProjectChanged;
    Project? CurrentProject { get; }
}

public partial class ProjectManager : ObservableObject, IProjectManager
{
    public ProjectManager(IOService ioService, PathService pathService)
    {
        _ioService = ioService;
        _pathService = pathService;
    }

    public void Save()
    {
        if (CurrentProject is null)
        {
            return;
        }

        _ioService.GetService<JsonIO>().Save(CurrentProject, UnixPath.Combine(CurrentProject.Path));
    }

    public bool Load(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        
        var projectSession = _ioService.GetService<JsonIO>().Load(UnixPath.Combine(path));
        if (projectSession is null)
        {
            CurrentProject = new Project(_pathService) { Name = "Warpunk: Emberfall" };
            return true;
        }

        ProjectChanging?.Invoke(this, projectSession);
        CurrentProject = projectSession;
        ProjectChanged?.Invoke(this, projectSession);
        return true;
    }

    public event EventHandler<Project>? ProjectChanging;
    public event EventHandler<Project>? ProjectChanged;
    public Project? CurrentProject { get; private set; } = null;

    private readonly IOService _ioService;
    private readonly PathService _pathService;
}