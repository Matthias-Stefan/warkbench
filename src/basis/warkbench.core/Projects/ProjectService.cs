using warkbench.src.basis.core.Common;
using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.core.Projects;

public class ProjectService(IProjectIoService projectIo, IPathService pathService, ILogger logger) : IProjectService
{
    public IProject CreateProject(string name)
    {
        var localPath = new LocalPath($"{name}{IProjectIoService.Extension}");
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocalPath = localPath,
            Version = Versions.CurrentProjectVersion,
            IsDirty = true
        };
        
        SaveProject(project);
        return project;
    }

    public IProject LoadProject(LocalPath localPath)
    {
        if (localPath.IsEmpty)
            throw new ArgumentException("[ProjectService] Project path cannot be null or empty.", nameof(localPath));

        var absolutePath = new AbsolutePath(
            UnixPath.Combine(pathService.ProjectPath.Value, localPath.Value)
        );

        var loadedProject = projectIo.Load<Project>(absolutePath);
        if (loadedProject is null)
        {
            var errorMsg = $"[ProjectService] Failed to load project. No valid project file found at: {absolutePath.Value}";
            logger.Error(errorMsg);
            throw new FileNotFoundException(errorMsg, absolutePath.Value);
        }

        logger.Info($"[ProjectService] Project '{loadedProject.Name}' loaded and activated from {absolutePath.Value}");
        return loadedProject;
    }

    public Task<IProject> LoadProjectAsync(LocalPath localPath)
    {
        return Task.FromResult(LoadProject(localPath));
    }

    public void SaveProject(IProject project)
    {
        var absolutePath = new AbsolutePath(
            UnixPath.Combine(pathService.ProjectPath.Value, project.LocalPath.Value)
        );
        
        if (project.IsDirty)
            projectIo.Save(project, absolutePath);

        project.IsDirty = false;
    }

    public void DeleteProject(IProject project)
    {
        var absolutePath = new AbsolutePath(
            UnixPath.Combine(pathService.ProjectPath.Value, project.LocalPath.Value)
        );
        
        if (Directory.Exists(absolutePath.Value))
            Directory.Delete(absolutePath.Value, true);
    }
}