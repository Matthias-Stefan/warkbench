using Newtonsoft.Json;
using warkbench.src.basis.core.Common;
using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Io;

public class WorldIoService(ILogger logger) : BaseIoService, IWorldIoService
{
    public void Save<T>(T value, AbsolutePath path) where T : class
    {
        EnsureExtension(path, IWorldIoService.Extension);
        
        if (value is not IWorld world)
        {
            var errorMsg = $"[WorldIoService] Save failed. Expected IWorld, but received {typeof(T).Name}.";
            logger.Error(errorMsg);
            throw new ArgumentException(errorMsg);
        }
        
        var directory = Path.GetDirectoryName(path.Value);
        if (string.IsNullOrEmpty(directory))
        {
            var errorMsg = $"[WorldIoService] Invalid save path: {path}";
            logger.Error(errorMsg);
            throw new IOException(errorMsg);    
        }
        
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        var json = JsonConvert.SerializeObject(value, JsonSettings);
        File.WriteAllText(path.Value, json);
    }

    public T? Load<T>(AbsolutePath path) where T : class
    {
        EnsureExtension(path, IWorldIoService.Extension);
        
        if (!File.Exists(path.Value)) 
            return null;

        var json = File.ReadAllText(path.Value);
        return JsonConvert.DeserializeObject<T>(json, JsonSettings);
    }

    public void PopulateWorld(AbsolutePath path, IWorld target)
    {
        EnsureExtension(path, IWorldIoService.Extension);
        
        if (!File.Exists(path.Value)) 
            return;

        var json = File.ReadAllText(path.Value);
        JsonConvert.PopulateObject(json, target, JsonSettings);
    }

    public IEnumerable<string> DiscoverWorlds(AbsolutePath searchPath, bool recursive = true)
    {
        if (!Directory.Exists(searchPath.Value))
            return [];
        
        var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        
        return Directory.EnumerateFiles(searchPath.Value, $"*{IWorldIoService.Extension}", option)
            .Select(path => UnixPath.GetFullPath(path)!);
    }
}