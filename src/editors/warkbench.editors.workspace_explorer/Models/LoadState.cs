namespace warkbench.project_explorer.Models;

/// <summary>Represents the current loading lifecycle state of an item.</summary>
public enum LoadState
{
    /// <summary>The item is known but not loaded into memory.</summary>
    NotLoaded,

    /// <summary>The item is currently being loaded.</summary>
    Loading,

    /// <summary>The item has been successfully loaded and is ready for use.</summary>
    Loaded,

    /// <summary>The item failed to load due to an error.</summary>
    Failed
}