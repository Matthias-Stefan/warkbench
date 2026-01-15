using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.basis.core.Common;

public class ProjectIoService(PathService pathService, ILogger logger) : IProjectIoService
{
    public void Save<T>(T value, string path) where T : class
    {
        if (value is not IProject project)
        {
            var errorMsg = $"[ProjectIoService] Save failed. Expected IProject, but received {typeof(T).Name}.";
            throw new ArgumentException(errorMsg);
        }
        
        var directory = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(directory))
            throw new IOException($"[ProjectIoService] Invalid save path: {path}");
        
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

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        PreserveReferencesHandling = PreserveReferencesHandling.All,
        Formatting = Formatting.Indented,
        ContractResolver = new ProjectIoContractResolver()
    };

    private class ProjectIoContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (prop.Writable) 
                return prop;
            
            var property = member as PropertyInfo;
            var hasPrivateSetter = property?.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;
            return prop;
        }
    }
}