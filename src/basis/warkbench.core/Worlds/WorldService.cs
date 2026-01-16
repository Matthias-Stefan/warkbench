using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Worlds;

public class WorldService(IProjectService projectService, IPathService pathService, ILogger logger) : IWorldService
{
    public void CreateWorld(string name)
    {
        var currentProject = projectService.ActiveProject;
        
        if (currentProject is null)
        {
            const string errorMsg = "Cannot create world: No project is currently active.";
            logger.Error(errorMsg);
            throw new InvalidOperationException(errorMsg);
        }
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("World name cannot be empty.", nameof(name));
        
        IWorld newWorld = new World
        {
            Id = Guid.NewGuid(),
            Name = name,
            LocalPath = pathService.GetRelativeLocalPath(pathService.ProjectPath, pathService.BasePath),
        };
        
        currentProject.AddWorld(newWorld);
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
        var currentProject = projectService.ActiveProject;
        
        if (currentProject is null)
        {
            const string errorMsg = "Cannot delete world: No project is currently active.";
            logger.Error(errorMsg);
            throw new InvalidOperationException(errorMsg);
        }

        var worldToDelete = currentProject.Worlds.FirstOrDefault(world => world.Id == worldId);
        
        if (worldToDelete is null)
        {
            logger.Warn($"DeleteWorld failed: World with ID '{worldId}' was not found in project '{currentProject.Name}'.");
            return;
        }
        
        if (ActiveWorld?.Id == worldId)
        {
            CloseWorld();
        }
        
        currentProject.RemoveWorld(worldToDelete);
        
        
        
        logger.Info($"Successfully removed world '{worldToDelete.Name}' from project.");
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
    
    public event Action<IWorld?>? ActiveWorldChanged;
    
    private IWorld? _activeWorld;
}