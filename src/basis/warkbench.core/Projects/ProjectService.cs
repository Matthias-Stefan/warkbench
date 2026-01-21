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

    public IProject LoadProject(string path)
    {
        var loadedProject = projectIo.Load<Project>(path);
        if (loadedProject is null)
        {
            var errorMsg = $"[ProjectService] Failed to load project. No valid project file found at: {path}";
            logger?.Error(errorMsg); 
        
            throw new FileNotFoundException(errorMsg, path);
        }

        logger?.Info($"[ProjectService] Project '{loadedProject.Name}' loaded and activated.");
        return loadedProject;
    }

    public Task<IProject> LoadProjectAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("[ProjectService] Project path cannot be null or empty.", nameof(path));

        try
        {
            // TODO: make Load awaitable 
            var loadedProject = projectIo.Load<Project>(path);
            if (loadedProject is null)
            {
                var errorMsg = $"[ProjectService] Failed to load project. No valid project file found at: {path}";
                logger.Error(errorMsg);
                throw new InvalidDataException(errorMsg);
            }

            logger.Info($"[ProjectService] Project '{loadedProject.Name}' loaded and activated from {path}");
            return Task.FromResult<IProject>(loadedProject);
        }
        catch (Exception ex)
        {
            logger.Error($"[ProjectService] Critical error loading project at {path}: {ex.Message}");
            throw;
        }
    }

    public void SaveProject(IProject project)
    {
        if (project.IsDirty)
            projectIo.Save(project, pathService.ProjectPath);

        project.IsDirty = false;
    }

    public void DeleteProject(IProject project)
    {
        var absolutePath = UnixPath.Combine(pathService.BasePath, project!.LocalPath);
        if (absolutePath == pathService.BasePath)
            return;

        if (Directory.Exists(absolutePath))
            Directory.Delete(absolutePath, true);
    }
}