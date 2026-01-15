using System.ComponentModel;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.interfaces.Projects;

/// <summary>
/// Defines the core data structure and state of a workbench project.
/// </summary>
public interface IProject : IIdentifiable, INotifyPropertyChanged
{
    /// <summary> Textual summary providing project context or metadata. </summary>
    string Description { get; }
    
    /// <summary> The relative folder name or path within the workspace. </summary>
    string LocalPath { get; }
    
    /// <summary> ISO timestamp of the initial project creation. </summary>
    DateTime CreatedAt { get; }

    /// <summary> ISO timestamp of the most recent persistence operation. </summary>
    DateTime LastModifiedAt { get; }
    
    /// <summary> Semantic version of the application used during project creation. </summary>
    string Version { get; }
    
    /// <summary> Flag indicating unsaved changes in the current session. </summary>
    bool IsDirty { get; set; }
    
    /// <summary> Collection of worlds associated with the project hierarchy. </summary>
    IEnumerable<IWorld> Worlds { get; }

    /// <summary> The currently focused or edited world in the workspace. </summary>
    IWorld ActiveWorld { get; set; }
    
    // TODO: later IPackage
    /// <summary> Registry of external assets and dependency packages. </summary>
    IEnumerable<object> Packages { get; }
    
    // TODO: later IBlueprint
    /// <summary> Collection of logic graphs and behavioral definitions. </summary>
    IEnumerable<object> Blueprints { get; }
    
    // TODO: later IProperty
    /// <summary> Set of global project parameters and configuration entries. </summary>
    IEnumerable<object> Properties { get; }
}