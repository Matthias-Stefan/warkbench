using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.interfaces.Paths;

/// <summary> Provides centralized access to standardized application and project-specific file system locations. </summary>
public interface IPathService
{
    /// <summary> Converts an absolute path into a normalized relative path starting from the workspace root. </summary>
    LocalPath GetRelativeLocalPath(AbsolutePath absolutePath, AbsolutePath relativeTo);
    
    /// <summary> Gets the absolute base directory (anchor) of the development workspace. </summary>
    AbsolutePath BasePath { get; }
    
    /// <summary> Gets the absolute root path of the Warkbench development environment. </summary>
    AbsolutePath ToolsPath { get; }
    
    /// <summary> Gets the output directory for generated game data and build artifacts. </summary>
    AbsolutePath DataPath { get; }
    
    /// <summary> Gets the directory containing raw source assets like textures, sounds, and models. </summary>
    AbsolutePath AssetsPath { get; }
    
    /// <summary> Gets the absolute path to the primary project configuration file (project.json). </summary>
    AbsolutePath ProjectsPath { get; }
}