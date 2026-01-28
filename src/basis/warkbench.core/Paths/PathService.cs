using warkbench.src.basis.interfaces.Logger;
using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.core.Paths;

public class PathService : IPathService
{
    public PathService(ILogger logger)
    {
        ToolsPath = FindToolsPath(AppContext.BaseDirectory, "warkbench");
        
        var parent = Directory.GetParent(ToolsPath.Value);
        if (parent is null)
        {
            const string errorMsg = "Fatal: Could not resolve BasePath.";
            logger.Error<PathService>(errorMsg);
            
            throw new DirectoryNotFoundException(errorMsg);
        }
        
        BasePath = new AbsolutePath(UnixPath.GetFullPath(parent.FullName));

        // --- CURRENT: Static structure where engine and game are siblings ---
        // In the future, these paths can be overridden dynamically by the 'ProjectService' 
        // when a specific project is loaded or created.
        const string gameFolderName = "warpunk.emberfall";
        DataPath = new AbsolutePath(UnixPath.Combine(BasePath.Value, gameFolderName, "data"));
        AssetsPath = new AbsolutePath(UnixPath.Combine(BasePath.Value, gameFolderName, "assets"));
        
        const string projectFolderName = "projects";
        ProjectsPath = new AbsolutePath(UnixPath.Combine(ToolsPath.Value, projectFolderName));
        
        logger.Info<PathService>($"Initialized. Base: {BasePath}");
        logger.Info<PathService>($"Initialized. Tools: {ToolsPath}");
        logger.Info<PathService>($"Initialized. Data: {DataPath}");
        logger.Info<PathService>($"Initialized. Assets: {AssetsPath}");
        logger.Info<PathService>($"Initialized. Projects: {ProjectsPath}");
    }

    public LocalPath GetRelativeLocalPath(AbsolutePath absolutePath, AbsolutePath relativeTo)
    {
        var relative = Path.GetRelativePath(relativeTo.Value, absolutePath.Value);
        return new LocalPath(UnixPath.ToUnix(relative));
    }

    public AbsolutePath BasePath { get; }
    public AbsolutePath ToolsPath { get; }
    public AbsolutePath DataPath { get; }
    public AbsolutePath AssetsPath { get; }
    public AbsolutePath ProjectsPath { get; }
    
    private static AbsolutePath FindToolsPath(string startPath, string targetName)
    {
        var current = new DirectoryInfo(startPath);
        while (current != null)
        {
            if (current.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
            {
                return new AbsolutePath(UnixPath.GetFullPath(current.FullName));
            }
            current = current.Parent;
        }
        return new AbsolutePath(UnixPath.GetFullPath(startPath));
    }
}