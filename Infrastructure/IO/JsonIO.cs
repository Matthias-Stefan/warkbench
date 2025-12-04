using System.IO;
using Newtonsoft.Json;
using warkbench.Models;


namespace warkbench.Infrastructure;
public class JsonIO : IOBase
{
    public JsonIO(PathService pathService)
    {
        PathService = pathService;
    }
    
    public void Save(object? value, string? path)
    {
        var json = JsonConvert.SerializeObject(value, JsonSettings);
        if (path != null)
        {
            File.WriteAllText(path, json);
        }
    }

    public T? Load<T>(string? path) where T : class
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            return null;
        }

        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json, JsonSettings);
    }
    
    public Project? Load(string? path)
    {
        var session = new Project(PathService);
        if (!File.Exists(path))
        {
            return session;
        }

        var json = File.ReadAllText(path);
        JsonConvert.PopulateObject(json, session, JsonSettings);

        session.IsDirty = false;
        return session;
    }
    
    private PathService PathService { get; }
    
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        PreserveReferencesHandling = PreserveReferencesHandling.All,
        Formatting = Formatting.Indented,
    };
}