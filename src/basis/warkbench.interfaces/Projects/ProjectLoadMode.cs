namespace warkbench.src.basis.interfaces.Projects;

public enum ProjectLoadMode
{
    /// <summary>Loads only the project manifest without loading any worlds.</summary>
    ManifestOnly,

    /// <summary>Loads the project and its startup or last active world.</summary>
    StartupWorld,

    /// <summary>Loads the project and all associated worlds.</summary>
    Full
}