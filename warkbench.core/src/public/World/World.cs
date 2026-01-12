using System;
using System.Collections.Generic;

namespace warkbench.core;
public class World
{
    public World(int tileSize = 32, int chunkSize = 32)
    {
        TileSize = tileSize;
        ChunkSize = chunkSize;
    }
    
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "New World";
    
    public int TileSize { get; }
    public int ChunkSize { get; }
    
    public int ChunkPixelSize => ChunkSize * ChunkSize;
    
    private Dictionary<(long x, long y), Chunk> _chunks = []; 
}