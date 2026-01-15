using warkbench.src.basis.core.Common;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.core.Projects;

public class ProjectService(IProjectIoService projectIo, IPathService pathService, ILogger logger) : IProjectService
{
    public IProject CreateProject(string name)
    {
        return new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocalPath = pathService.GetRelativeLocalPath(pathService.ProjectPath, pathService.BasePath),
            Version = Versions.CurrentProjectVersion
        };
    }

    public void LoadProject(string path)
    {
        var loaded = projectIo.Load<Project>(path);
    
        if (loaded is null)
        {
            var errorMsg = $"[ProjectService] Failed to load project. No valid project file found at: {path}";
            logger?.Error(errorMsg); 
        
            throw new FileNotFoundException(errorMsg, path);
        }

        ActiveProject = loaded;
        logger?.Info($"[ProjectService] Project '{ActiveProject.Name}' loaded and activated.");
    }

    public void SaveProject()
    {
        if (ActiveProject == null) 
            return;

        projectIo.Save(ActiveProject, pathService.ProjectPath);
        ActiveProject.IsDirty = false;
    }

    public void CloseProject()
    {
        ActiveProject = null;
    }

    public void DeleteProject(string path)
    {
        if (ActiveProject is null)
            return;
        
        if (ActiveProject?.LocalPath == path)
            CloseProject();

        var absolutePath = UnixPath.Combine(pathService.BasePath, ActiveProject!.LocalPath);
        if (absolutePath == pathService.BasePath)
            return;

        if (Directory.Exists(absolutePath))
            Directory.Delete(absolutePath, true);
    }

    public IProject? ActiveProject
    {
        get => _activeProject;
        private set
        {
            if (_activeProject == value) 
                return;
            
            _activeProject = value;
            ActiveProjectChanged?.Invoke(_activeProject);
        }
    }

    public event Action<IProject?>? ActiveProjectChanged;

    private IProject? _activeProject;
}