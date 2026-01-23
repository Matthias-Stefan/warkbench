using Newtonsoft.Json;
using warkbench.src.basis.core.App;
using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;

namespace warkbench.src.basis.core.Io;

public class AppStateIoService(ILogger logger) : BaseIoService, IAppStateIoService
{
    public IAppState? Load()
    {
        var path = GetAppStateFilePath();

        if (!File.Exists(path))
            return null;

        try
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<AppState>(json, JsonSettings);
        }
        catch (Exception ex)
        {
            logger.Error($"[AppStateIoService] Load failed for '{path}'. {ex.Message}");
            return null;
        }
    }

    public void Save(IAppState state)
    {
        var path = GetAppStateFilePath();
        var dir  = Path.GetDirectoryName(path);

        if (string.IsNullOrEmpty(dir))
        {
            var errorMsg = $"[AppStateIoService] Invalid save path: {path}";
            logger.Error(errorMsg);
            throw new IOException(errorMsg);
        }

        try
        {
            Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(state, JsonSettings);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            logger.Error($"[AppStateIoService] Save failed for '{path}'. {ex.Message}");
            throw;
        }
    }
    
    private static string GetAppStateFilePath()
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(baseDir, AppName, FileName);
    }
    
    private const string AppName  = "Warkbench";
    private const string FileName = "appstate.json";
}