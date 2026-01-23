using Newtonsoft.Json;
using warkbench.src.basis.core.Common;
using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.core.Io;

public class ProjectIoService(ILogger logger) : BaseIoService, IProjectIoService
{
    public void Save<T>(T value, AbsolutePath path) where T : class
    {
        EnsureExtension(path, IProjectIoService.Extension);
        
        if (value is not IProject project)
        {
            var errorMsg = $"[ProjectIoService] Save failed. Expected IProject, but received {typeof(T).Name}.";
            logger.Error(errorMsg);
            throw new ArgumentException(errorMsg);
        }
        
        var directory = Path.GetDirectoryName(path.Value);
        if (string.IsNullOrEmpty(directory))
        {
            var errorMsg = $"[ProjectIoService] Invalid save path: {path}";
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
        EnsureExtension(path, IProjectIoService.Extension);
        
        if (!File.Exists(path.Value)) 
            return null;

        var json = File.ReadAllText(path.Value);
        return JsonConvert.DeserializeObject<T>(json, JsonSettings);
    }

    public void PopulateProject(AbsolutePath path, IProject target)
    {
        EnsureExtension(path, IProjectIoService.Extension);
        
        if (!File.Exists(path.Value)) 
            return;

        var json = File.ReadAllText(path.Value);
        JsonConvert.PopulateObject(json, target, JsonSettings);
    }
}