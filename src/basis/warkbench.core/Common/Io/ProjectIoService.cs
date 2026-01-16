using Newtonsoft.Json;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.core.Common;

public class ProjectIoService(ILogger logger) : BaseIoService, IProjectIoService
{
    public void Save<T>(T value, string path) where T : class
    {
        if (value is not IProject project)
        {
            var errorMsg = $"[ProjectIoService] Save failed. Expected IProject, but received {typeof(T).Name}.";
            logger.Error(errorMsg);
            throw new ArgumentException(errorMsg);
        }
        
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(directory))
        {
            var errorMsg = $"[ProjectIoService] Invalid save path: {path}";
            logger.Error(errorMsg);
            throw new IOException(errorMsg);    
        }
        
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        var json = JsonConvert.SerializeObject(value, JsonSettings);
        File.WriteAllText(path, json);
    }

    public T? Load<T>(string path) where T : class
    {
        if (!File.Exists(path)) 
            return null;

        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json, JsonSettings);
    }

    public void PopulateProject(string path, IProject target)
    {
        if (!File.Exists(path)) 
            return;

        var json = File.ReadAllText(path);
        JsonConvert.PopulateObject(json, target, JsonSettings);
    }

    public IEnumerable<string> DiscoverProjects(string searchPath, bool recursive = true)
    {
        var normalizedSearchPath = UnixPath.GetFullPath(searchPath);
    
        if (!Directory.Exists(normalizedSearchPath))
            return [];

        var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        return Directory.EnumerateFiles(normalizedSearchPath, $"*{IProjectIoService.Extension}", option)
            .Select(path => UnixPath.GetFullPath(path)!);
    }
}