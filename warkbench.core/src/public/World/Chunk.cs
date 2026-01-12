using System.Dynamic;

namespace warkbench.core;
public class Chunk
{
    public Chunk(long x, long y, int size)
    {
        ChunkX = x;
        ChunkY = y;
        Size = size;

        _tiles = new int[size * size];
    }

    public void SetTile(int localX, int localY, int tileId)
    {
        if (IsValidCoordinate(localX, localY))
        {
            int index = GetIndex(localX, localY);
            _tiles[index] = tileId;
        }
    }

    public int GetTile(int localX, int localY)
    {
        if (!IsValidCoordinate(localX, localY))
            return InvalidTile;
        
        var index = GetIndex(localX, localY);
        return _tiles[index];
    }

    private int GetIndex(int x, int y)
    {
        // Formel: Zeile * Breite + Spalte
        return (y * Size) + x;
    }
    
    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < Size && y >= 0 && y < Size;
    }
    
    public long ChunkX { get; }
    public long ChunkY { get; }
    public int Size { get; }

    public const int InvalidTile = -1;

    private readonly int[] _tiles;
}