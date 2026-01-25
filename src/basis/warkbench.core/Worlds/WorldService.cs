using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Worlds;

public class WorldService(
    IProjectService projectService, 
    IPathService pathService, 
    ILogger logger,
    IWorldIoService worldIo) : IWorldService
{
    public IWorld CreateWorld(IProject project, string name, int tileSize, int chunkResolution)
    {
        var worldsFolderPath = project.WorldsFolderPath;
        
        IWorld newWorld = new World
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocalPath = new LocalPath(UnixPath.Combine(worldsFolderPath.Value, $"{name}{IWorldIoService.Extension}")),
            TileSize = tileSize,
            ChunkResolution = chunkResolution ,
            IsDirty = true
        };
        
        SaveWorld(newWorld);
        project.AddWorld(newWorld);
        return newWorld;
    }

    public async Task<IWorld> CreateWorldAsync(IProject project, string name, int tileSize, int chunkResolution)
    {
        var worldsFolderPath = project.WorldsFolderPath;
        
        IWorld newWorld = new World
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocalPath = new LocalPath(UnixPath.Combine(worldsFolderPath.Value, $"{name}{IWorldIoService.Extension}")),
            TileSize = tileSize,
            ChunkResolution = chunkResolution,
            IsDirty = true
        };
        
        await SaveWorldAsync(newWorld); 
        project.AddWorld(newWorld);
        return newWorld;
    }

    public IWorld? LoadWorld(Guid worldId)
    {
        throw new NotImplementedException();
    }

    public async Task<IWorld>? LoadWorldAsync(Guid worldId)
    {
        throw new NotImplementedException();
    }

    public void SaveWorld(IWorld world)
    {
        var absolutePath = new AbsolutePath(
            UnixPath.Combine(pathService.ProjectsPath.Value, world.LocalPath.Value)
        );
        
        if (world.IsDirty)
            worldIo.Save(world, absolutePath);

        world.IsDirty = false;
    }

    public Task SaveWorldAsync(IWorld world)
    {
        var absolutePath = new AbsolutePath(
            UnixPath.Combine(pathService.ProjectsPath.Value, world.LocalPath.Value)
        );
        
        if (world.IsDirty)
            return worldIo.SaveAsync(world, absolutePath);

        world.IsDirty = false;
        return Task.CompletedTask;
    }

    public void SaveAllDirty(IEnumerable<IWorld> worlds)
    {
        foreach (var world in worlds)
        {
            if (world.IsDirty)
                SaveWorld(world);
        }
    }

    public Task SaveAllDirtyAsync(IEnumerable<IWorld> worlds)
    {
        foreach (var world in worlds)
        {
            if (world.IsDirty)
                SaveWorldAsync(world);
        }
        
        return Task.CompletedTask;
    }
    
    public void DeleteWorld(IWorld world)
    {
        throw new NotImplementedException();
    }
}