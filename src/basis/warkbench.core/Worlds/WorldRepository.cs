using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Worlds;

public class WorldRepository : IWorldRepository
{
    public bool TryGet(LocalPath path, out IWorld? world)
        => _worlds.TryGetValue(path, out world);

    public void Add(LocalPath path, IWorld world)
        => _worlds[path] = world;

    public bool ContainsKey(LocalPath path)
        => _worlds.ContainsKey(path);
    
    public bool Remove(LocalPath path)
        => _worlds.Remove(path);    

    public void Clear()
        => _worlds.Clear();

    public IReadOnlyCollection<IWorld> Worlds => _worlds.Values;

    private readonly Dictionary<LocalPath, IWorld> _worlds = [];
}