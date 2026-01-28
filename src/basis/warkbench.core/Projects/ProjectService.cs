using warkbench.src.basis.core.Common;
using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Logger;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.core.Projects;

public class ProjectService(
    IProjectIoService projectIo, 
    IPathService pathService, 
    ILogger logger) : IProjectService
{
    public async Task<IProject> CreateProjectAsync(string name)
    {
        CreateProjectStructure(name);

        var localPath = new LocalPath(UnixPath.Combine(name, $"{name}{IProjectIoService.Extension}"));
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocalPath = localPath,
            Version = Versions.CurrentProjectVersion,
            IsDirty = true
        };
        
        await SaveProjectAsync(project);
        return project;
    }

    public async Task<IProject> LoadProjectAsync(AbsolutePath absolutePath)
    {
        if (absolutePath.IsEmpty)
        {
            const string errorMsg = "Project path cannot be null or empty.";
            logger.Error<ProjectService>(errorMsg);
            throw new ArgumentException(errorMsg, nameof(absolutePath));
        }
        
        var loadedProject = await projectIo.LoadAsync(absolutePath);
        if (loadedProject is null)
        {
            var errorMsg = $"Failed to load project. No valid project file found at: {absolutePath.Value}";
            logger.Error<ProjectService>(errorMsg);
            throw new FileNotFoundException(errorMsg, absolutePath.Value);
        }

        logger.Info<ProjectService>($"Project '{loadedProject.Name}' loaded and activated from {absolutePath.Value}");
        return loadedProject;
    }

    public async Task SaveProjectAsync(IProject project)
    {
        var absolutePath = new AbsolutePath(
            UnixPath.Combine(pathService.ProjectsPath.Value, project.LocalPath.Value)
        );
        
        if (project.IsDirty)
            await projectIo.SaveAsync(project, absolutePath);

        project.IsDirty = false;
    }

    public Task DeleteProjectAsync(IProject project)
    {
        var absolutePath = new AbsolutePath(
            UnixPath.Combine(pathService.ProjectsPath.Value, project.LocalPath.Value)
        );
        
        if (Directory.Exists(absolutePath.Value))
            Directory.Delete(absolutePath.Value, true);
        
        return Task.CompletedTask;
    }

    private void CreateProjectStructure(string name)
    {
        var projectFolderPath = UnixPath.Combine(pathService.ProjectsPath.Value, name);

        List<AbsolutePath> paths = [
            new (projectFolderPath),
            new (UnixPath.Combine(projectFolderPath, IProject.WorldsFolderName)),
            new (UnixPath.Combine(projectFolderPath, IProject.ScenesFolderName)),
            new (UnixPath.Combine(projectFolderPath, IProject.PackagesFolderName)),
            new (UnixPath.Combine(projectFolderPath, IProject.BlueprintsFolderName)),
            new (UnixPath.Combine(projectFolderPath, IProject.PropertiesFolderName)),
        ];
        
        logger.Info<ProjectService>($"Creating project directory structure for '{name}'.");
        
        try
        {
            foreach (var path in paths)
            {
                Directory.CreateDirectory(path.Value);
                logger.Info<ProjectService>($"Created directory: '{path.Value}'.");
            }
        }
        catch (IOException ex)
        {
            logger.Error<ProjectService>("Failed to create project directory structure.", ex);
            throw;
        }
    }
}