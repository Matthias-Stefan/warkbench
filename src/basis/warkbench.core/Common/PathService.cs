using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.core.Common;

public class PathService : IPathService
{
    public PathService(ILogger logger)
    {
        ToolsPath = FindToolsPath(AppContext.BaseDirectory, "warkbench");
        
        var parent = Directory.GetParent(ToolsPath);
        if (parent is null)
        {
            const string errorMsg = "Fatal: Could not resolve BasePath.";
            logger.Error(errorMsg);
            
            throw new DirectoryNotFoundException(errorMsg);
        }
        
        BasePath = UnixPath.GetFullPath(parent.FullName)!;

        // --- CURRENT: Static structure where engine and game are siblings ---
        // In the future, these paths can be overridden dynamically by the 'ProjectService' 
        // when a specific project is loaded or created.
        const string projectFolderName = "warpunk.emberfall";
        
        DataPath = UnixPath.Combine(BasePath, projectFolderName, "data");
        AssetsPath = UnixPath.Combine(BasePath, projectFolderName, "assets");
        ProjectPath = UnixPath.Combine(BasePath, projectFolderName, "data");
        
        logger.Info($"PathService initialized. Base: {BasePath}");
    }

    public string GetRelativeLocalPath(string absolutePath, string relativeTo)
    {
        var relative = Path.GetRelativePath(relativeTo, absolutePath);
        return UnixPath.ToUnix(relative);
    }

    public string BasePath { get; }
    public string ToolsPath { get; }
    public string DataPath { get; }
    public string AssetsPath { get; }
    public string ProjectPath { get; }
    
    private static string FindToolsPath(string startPath, string targetName)
    {
        var current = new DirectoryInfo(startPath);
        while (current != null)
        {
            if (current.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
            {
                return UnixPath.GetFullPath(current.FullName)!;
            }
            current = current.Parent;
        }
        return UnixPath.GetFullPath(startPath)!;
    }
}