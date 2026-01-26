using System.ComponentModel;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Scenes;

namespace warkbench.src.basis.interfaces.Worlds;

/// <summary> Represents the persistent physical universe of a project, managing spatial chunks and associated content scenes. </summary>
public interface IWorld : IIdentifiable, IDirtyable, INotifyPropertyChanged
{
    /// <summary> Textual summary providing world context or metadata. </summary>
    string Description { get; }
    
    /// <summary> ISO timestamp of the initial world creation. </summary>
    DateTime CreatedAt { get; }

    /// <summary> ISO timestamp of the most recent persistence operation. </summary>
    DateTime LastModifiedAt { get; }
    
    /// <summary> The relative folder name or path within the workspace. </summary>
    LocalPath LocalPath { get; }
    
    /// <summary> Size in pixels of a single tile. </summary>
    int TileSize { get; }
    
    /// <summary> Tiles per side in one Chunk (e.g. 16 means a 16x16 grid). </summary>
    int ChunkResolution { get; set; }
    
    /// <summary> Computed property for convenience. </summary>
    int TotalTilesPerChunk => ChunkResolution * ChunkResolution;
    
    // ---------------------------- TODO:
    
    // TODO: IChunk
    /// <summary> Gets the collection of spatial segments containing terrain and static data. </summary>
    IEnumerable<object> Chunks { get; }

    /// <summary> Gets the collection of all content scenes (layers) associated with this world. </summary>
    IEnumerable<IScene> Scenes { get; }

    /// <summary> Gets or sets the currently active scene layer for editing. </summary>
    IScene? ActiveScene { get; set; }

    /// <summary> Gets a list of currently loaded or visible scenes in the editor. </summary>
    IEnumerable<IScene> LoadedScenes { get; }
}