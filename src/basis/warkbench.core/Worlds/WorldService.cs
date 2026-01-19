using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Worlds;

public class WorldService : IWorldService, IDisposable
{
    public WorldService(IProjectService projectService, IPathService pathService, ILogger logger)
    {
        _projectService = projectService;
        _pathService = pathService;
        _logger = logger;

        _projectService.ActiveProjectChanged += OnProjectChanged;
    }
    
    public void Dispose()
    {
        _projectService.ActiveProjectChanged -= OnProjectChanged;
    }

    public IWorld CreateWorld(string name, int tileSize, int chunkResolution)
    {
        var currentProject = _projectService.ActiveProject;
        
        if (currentProject is null)
        {
            const string errorMsg = "[WorldService] Cannot create world: No project is currently active.";
            _logger.Error(errorMsg);
            throw new InvalidOperationException(errorMsg);
        }
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("[WorldService] World name cannot be empty.", nameof(name));
        
        IWorld newWorld = new World
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocalPath = _pathService.GetRelativeLocalPath(_pathService.ProjectPath, _pathService.BasePath),
            TileSize = tileSize,
            ChunkResolution = chunkResolution  
        };
        
        currentProject.AddWorld(newWorld);
        return newWorld;
    }

    public void LoadWorld(Guid worldId)
    {
        throw new NotImplementedException();
    }

    public void SaveWorld()
    {
        throw new NotImplementedException();
    }

    public void CloseWorld()
    {
        ActiveWorld = null;
    }

    public void DeleteWorld(Guid worldId)
    {
        var currentProject = _projectService.ActiveProject;
        
        if (currentProject is null)
        {
            const string errorMsg = "[WorldService] Cannot delete world: No project is currently active.";
            _logger.Error(errorMsg);
            throw new InvalidOperationException(errorMsg);
        }

        var worldToDelete = currentProject.Worlds.FirstOrDefault(world => world.Id == worldId);
        
        if (worldToDelete is null)
        {
            _logger.Warn($"[WorldService] DeleteWorld failed: World with ID '{worldId}' was not found in project '{currentProject.Name}'.");
            return;
        }
        
        if (ActiveWorld?.Id == worldId)
        {
            CloseWorld();
        }
        
        currentProject.RemoveWorld(worldToDelete);
        
        _logger.Info($"[WorldService] Successfully removed world '{worldToDelete.Name}' from project.");
    }

    public IWorld? ActiveWorld
    {
        get => _activeWorld;
        private set
        {
            if (_activeWorld == value) 
                return;
            
            _activeWorld = value;
            ActiveWorldChanged?.Invoke(_activeWorld);
        }
    }

    private void OnProjectChanged(IProject? project)
    {
        if (project is null)
        {
            _logger.Info("[WorldService] Project closed. Clearing active world...");
            ActiveWorld = null;
            ActiveWorldChanged?.Invoke(null);
            return;
        }
        
        _logger.Info($"[WorldService] Project '{project.Name}' opened. Syncing world list...");



    }


    public event Action<IWorld?>? ActiveWorldChanged;
    
    private IWorld? _activeWorld;
    
    private IProjectService _projectService;
    private IPathService _pathService;
    private ILogger _logger;
}