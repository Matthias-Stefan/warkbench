using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Worlds;

public class WorldService : IWorldService
{
    public WorldService(
        ILogger logger,
        IPathService pathService, 
        IWorldIoService worldIo,
        IWorldRepository worldRepository)
    {
        _logger = logger;
        _pathService = pathService;
        _worldIoService = worldIo;
        _worldRepository = worldRepository;
    }

    public async Task<IWorld> CreateWorldAsync(IProject project, string name, int tileSize, int chunkResolution)
    {
        // 1) Build project-relative world path (e.g. "worlds/<name>.wbworld")
        var worldPath = new LocalPath(
            UnixPath.Combine(project.LocalPath.Parent().Value, IProject.WorldsFolderName, $"{name}{IWorldIoService.Extension}")
        );

        if (project.Worlds.Contains(worldPath))
        {
            var errorMsg = $"World already exists in project: {worldPath.Value}";
            _logger.Error<WorldService>(errorMsg);
            throw new InvalidOperationException(errorMsg);
        }
        
        var newWorld = new World
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocalPath = worldPath,
            TileSize = tileSize,
            ChunkResolution = chunkResolution,
            IsDirty = true
        };

        // 2) Register loaded instance (cache/registry)
        _worldRepository.Add(worldPath, newWorld);

        // 3) Persist world file first
        await SaveWorldAsync(newWorld).ConfigureAwait(false);
        
        // 4) Update project manifest
        project.AddWorld(worldPath);
        
        return newWorld;
    }

    public async Task<IWorld> LoadWorldAsync(IProject project, LocalPath worldPath)
    {
        var absolutePath = new AbsolutePath(
            UnixPath.Combine(_pathService.ProjectsPath.Value, worldPath.Value)
        );
        
        var world = await _worldIoService.LoadAsync(absolutePath);
        if (world is not null) 
            return world;
        
        var errorMsg = $"Loading world {absolutePath.Value} failed";
        _logger.Error<WorldService>(errorMsg);
        throw new InvalidOperationException(errorMsg);

    }

    public async Task SaveWorldAsync(IWorld world)
    {
        var absolutePath = new AbsolutePath(
            UnixPath.Combine(_pathService.ProjectsPath.Value, world.LocalPath.Value)
        );
        
        if (world.IsDirty)
            await _worldIoService.SaveAsync(world, absolutePath);

        world.IsDirty = false;
    }

    public Task SaveAllDirtyAsync(IEnumerable<IWorld> worlds)
    {
        throw new NotImplementedException();
    }

    public Task DeleteWorldAsync(IWorld world)
    {
        throw new NotImplementedException();
    }

    private readonly ILogger _logger;
    private readonly IPathService _pathService;
    private readonly IWorldIoService _worldIoService;
    private readonly IWorldRepository _worldRepository;
}