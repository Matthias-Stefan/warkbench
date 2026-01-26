using System.ComponentModel;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.interfaces.Projects;

/// <summary>
/// Defines the warkbench.core data structure and state of a workbench project.
/// </summary>
public interface IProject : IIdentifiable, IDirtyable, INotifyPropertyChanged
{
    /// <summary>Adds a world reference (project-relative path) to the project manifest.</summary>
    void AddWorld(LocalPath worldPath);

    /// <summary>Removes a world reference (project-relative path) from the project manifest.</summary>
    void RemoveWorld(LocalPath worldPath);
    
    /// <summary>Gets the project file path relative to the projects root.</summary>
    LocalPath LocalPath { get; }

    /// <summary>Gets a textual description providing project context or metadata.</summary>
    string Description { get; }

    /// <summary>Gets the semantic version of the application used during project creation.</summary>
    string Version { get; }
    
    /// <summary>Gets the ISO timestamp of the initial project creation.</summary>
    DateTime CreatedAt { get; }

    /// <summary>Gets the ISO timestamp of the most recent persistence operation.</summary>
    DateTime LastModifiedAt { get; }
    
    /// <summary>Gets or sets whether the project manifest has unsaved changes.</summary>
    bool IsDirty { get; set; }
    
    /// <summary>Gets the collection of world references associated with the project.</summary>
    IReadOnlyList<LocalPath> Worlds { get; }

    /// <summary>Gets or sets the project-relative path of the last active world.</summary>
    LocalPath? ActiveWorldPath { get; set; }
    
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