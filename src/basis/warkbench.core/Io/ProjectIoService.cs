using Newtonsoft.Json;
using warkbench.src.basis.core.Projects;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.core.Io;

public class ProjectIoService(ILogger logger) : BaseIoService, IProjectIoService
{
    public async Task<IProject?> LoadAsync(AbsolutePath path)
    {
        EnsureExtension(path, IProjectIoService.Extension);
        
        var filePath = path.Value;
        
        if (!File.Exists(path.Value)) 
            return null;

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<Project>(json, JsonSettings);
        }
        catch (Exception ex)
        {
            logger.Error<ProjectIoService>($"Load failed for '{filePath}'.", ex);
            return null;
        }
    }

    public async Task SaveAsync(IProject value, AbsolutePath path)
    {
        EnsureExtension(path, IProjectIoService.Extension);
        
        var filePath = path.Value;
        var dir = Path.GetDirectoryName(filePath);
        
        if (string.IsNullOrEmpty(dir))
        {
            logger.Warn<ProjectIoService>($"Invalid save path: '{filePath}'.");
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
            logger.Error<ProjectIoService>($"Save failed for '{filePath}'.", ex);
            throw;
        }
    }

    public async Task PopulateProjectAsync(AbsolutePath path, IProject target)
    {
        EnsureExtension(path, IProjectIoService.Extension);
        
        var filePath = path.Value;
        
        if (!File.Exists(filePath)) 
            return;

        var json = await File.ReadAllTextAsync(filePath);
        JsonConvert.PopulateObject(json, target, JsonSettings);
    }
}