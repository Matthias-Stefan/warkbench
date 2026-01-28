using Newtonsoft.Json;
using warkbench.src.basis.core.Paths;
using warkbench.src.basis.core.Worlds;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Logger;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Io;

public class WorldIoService(ILogger logger) : BaseIoService, IWorldIoService
{
    public async Task<IWorld?> LoadAsync(AbsolutePath path)
    {
        EnsureExtension(path, IWorldIoService.Extension);
        
        var filePath = path.Value;
        
        if (!File.Exists(path.Value)) 
            return null;

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<World>(json, JsonSettings);
        }
        catch (Exception ex)
        {
            logger.Error<WorldIoService>($"Load failed for '{filePath}'.", ex);
            return null;
        }
    }

    public async Task SaveAsync(IWorld value, AbsolutePath path)
    {
        EnsureExtension(path, IWorldIoService.Extension);
        
        var filePath = path.Value;
        var dir = Path.GetDirectoryName(filePath);
        
        if (string.IsNullOrEmpty(dir))
        {
            logger.Warn<WorldIoService>($"Invalid save path: '{filePath}'.");
            return;   
        }
        
        try
        {
            Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(value, JsonSettings);
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception ex)
        {
            logger.Error<WorldIoService>($"Save failed for '{filePath}'.", ex);
            throw;
        }
    }

    public async Task PopulateWorldAsync(AbsolutePath path, IWorld target)
    {
        EnsureExtension(path, IWorldIoService.Extension);
        
        var filePath = path.Value;
        
        if (!File.Exists(filePath)) 
            return;

        var json = await File.ReadAllTextAsync(filePath);
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