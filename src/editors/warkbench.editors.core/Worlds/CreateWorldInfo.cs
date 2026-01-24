namespace warkbench.src.editors.core.Worlds;

/// <summary>Contains user-defined data required to create a new world.</summary>
public sealed class CreateWorldInfo
{
    /// <summary>The display name of the world.</summary>
    public required string WorldName { get; init; }

    /// <summary>The target directory where the world will be created.</summary>
    public required string WorldPath { get; init; }

    /// <summary>The size of a single tile in world units.</summary>
    public required int TileSize { get; init; }

    /// <summary>The number of tiles per world chunk.</summary>
    public required int ChunkSize { get; init; }
}