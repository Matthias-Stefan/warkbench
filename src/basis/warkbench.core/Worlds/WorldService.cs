using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Worlds;

public class WorldService(IProjectService projectService, IPathService pathService, ILogger logger) : IWorldService
{
    public IWorld CreateWorld(IProject project, string name, int tileSize, int chunkResolution)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("[WorldService] World name cannot be empty.", nameof(name));
        
        IWorld newWorld = new World
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocalPath = pathService.GetRelativeLocalPath(pathService.ProjectPath, pathService.BasePath),
            TileSize = tileSize,
            ChunkResolution = chunkResolution  
        };
        
        project.AddWorld(newWorld);
        return newWorld;
    }

    public IWorld LoadWorld(Guid worldId)
    {
        throw new NotImplementedException();
    }

    public void SaveWorld(IWorld world)
    {
        throw new NotImplementedException();
    }

    public void SaveAllDirty(IEnumerable<IWorld> worlds)
    {
        throw new NotImplementedException();
    }

    public Task SaveAllDirtyAsync(IEnumerable<IWorld> worlds)
    {
        throw new NotImplementedException();
    }

    public void DeleteWorld(IWorld world)
    {
        throw new NotImplementedException();
    }
}