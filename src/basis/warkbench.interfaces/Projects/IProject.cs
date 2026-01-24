using System.ComponentModel;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.interfaces.Projects;

/// <summary>
/// Defines the warkbench.core data structure and state of a workbench project.
/// </summary>
public interface IProject : IIdentifiable, INotifyPropertyChanged
{
    /// <summary> Registers a new world instance within the project's collection. </summary>
    void AddWorld(IWorld world);
    
    /// <summary> Removes an existing world instance from the project's collection. </summary>
    void RemoveWorld(IWorld world);
    
    /// <summary> The relative folder name or path within the workspace. </summary>
    LocalPath LocalPath { get; }
    
    /// <summary> Textual summary providing project context or metadata. </summary>
    string Description { get; }
    
    /// <summary> Semantic version of the application used during project creation. </summary>
    string Version { get; }
    
    /// <summary> ISO timestamp of the initial project creation. </summary>
    DateTime CreatedAt { get; }

    /// <summary> ISO timestamp of the most recent persistence operation. </summary>
    DateTime LastModifiedAt { get; }
    
    /// <summary> Flag indicating unsaved changes in the current session. </summary>
    bool IsDirty { get; set; }
    
    /// <summary> Collection of worlds associated with the project hierarchy. </summary>
    IEnumerable<IWorld> Worlds { get; }

    /// <summary> The currently focused or edited world in the workspace. </summary>
    IWorld? ActiveWorld { get; set; }
    
    // TODO: later IPackage
    /// <summary> Registry of external assets and dependency packages. </summary>
    IEnumerable<object> Packages { get; }
    
    // TODO: later IBlueprint
    /// <summary> Collection of logic graphs and behavioral definitions. </summary>
    IEnumerable<object> Blueprints { get; }
    
    // TODO: later IProperty
    /// <summary> Set of global project parameters and configuration entries. </summary>
    IEnumerable<object> Properties { get; }
    
    /// <summary>Directory name used to store world data.</summary>
    const string WorldsFolderName = "worlds";

    /// <summary>Directory name used to store scene data.</summary>
    const string ScenesFolderName = "scenes";

    /// <summary>Directory name used to store package definitions and related assets.</summary>
    const string PackagesFolderName = "packages";

    /// <summary>Directory name used to store blueprint definitions.</summary>
    const string BlueprintsFolderName = "blueprints";

    /// <summary>Directory name used to store project property data.</summary>
    const string PropertiesFolderName = "properties";
}