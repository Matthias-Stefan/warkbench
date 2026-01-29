using Microsoft.VisualBasic;
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

    public async Task RenameProjectAsync(IProject project, string name)
    {
        var newName = Path.GetFileNameWithoutExtension(name);
        
        var oldDirectory = new AbsolutePath(
            UnixPath.Combine(pathService.ProjectsPath.Value, Path.GetDirectoryName(project.LocalPath.Value))
        );

        var parentDirectory = UnixPath.GetDirectoryName(oldDirectory.Value);
        var newDirectory = new AbsolutePath(
            UnixPath.Combine(parentDirectory, newName)
        );
    
        var tempDirectory = new AbsolutePath(
            UnixPath.Combine(parentDirectory, $"{newName}.temp_{Guid.NewGuid()}")
        );
        
        try
        {
            // 1. Copy all files to temporary directory
            await Task.Run(() => CopyDirectoryAsync(oldDirectory.Value, tempDirectory.Value));

            // 2. Move temp directory to final name (atomic if same drive)
            await Task.Run(() => Directory.Move(tempDirectory.Value, newDirectory.Value));
            
            // 3. Rename project file
            var oldProjectFile = UnixPath.Combine(newDirectory.Value, project.Name);
            var newProjectFile = UnixPath.Combine(newDirectory.Value, $"{newName}{IProjectIoService.Extension}");
            await Task.Run(() => File.Move(oldProjectFile, newProjectFile));
            
            // 4. Update project object name
            project.Rename(name);

            // 5. Persist new project data
            await SaveProjectAsync(project);
            
            // 6. Delete old project directory
            await Task.Run(() => Directory.Delete(oldDirectory.Value, recursive: true));
        }
        catch
        {
            // Rollback on failure
            if (Directory.Exists(tempDirectory.Value))
                await Task.Run(() => Directory.Delete(tempDirectory.Value, recursive: true));

            if (Directory.Exists(newDirectory.Value))
                await Task.Run(() => Directory.Delete(newDirectory.Value, recursive: true));

            throw;
        }
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
    
    /// <summary>Recursively copies all files and folders from the source directory to the destination.</summary>
    private async Task CopyDirectoryAsync(string source, string destination)
    {
        // Create the root destination directory
        Directory.CreateDirectory(destination);

        // Create all subdirectories (including empty ones)
        foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(source, dir);
            var destDir = Path.Combine(destination, relativePath);
            Directory.CreateDirectory(destDir);
        }

        // Copy all files
        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(source, file);
            var destFile = Path.Combine(destination, relativePath);

            // Ensure the folder exists (redundant if directories were already created)
            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);

            // Copy the file without overwriting existing files
            File.Copy(file, destFile, overwrite: false);
        }
    }
}