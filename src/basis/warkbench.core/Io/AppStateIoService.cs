using Newtonsoft.Json;
using warkbench.src.basis.core.App;
using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.core.Io;

public class AppStateIoService(ILogger logger) : BaseIoService, IAppStateIoService
{
    public IAppState? Load(AbsolutePath path)
    {
        var filePath = path.Value;

        if (!File.Exists(filePath))
            return null;

        try
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<AppState>(json, JsonSettings);
        }
        catch (Exception ex)
        {
            logger.Error<AppStateIoService>($"Load failed for '{filePath}'.", ex);
            return null;
        }
    }

    public async Task<IAppState?> LoadAsync(AbsolutePath path)
    {
        var filePath = path.Value;

        if (!File.Exists(filePath))
            return null;

        try
        {
            var json = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<AppState>(json, JsonSettings);
        }
        catch (Exception ex)
        {
            logger.Error<AppStateIoService>($"LoadAsync failed for '{filePath}'.", ex);
            return null;
        }
    }
    
    public void Save(IAppState value, AbsolutePath path)
    {
        ArgumentNullException.ThrowIfNull(value);

        var filePath = path.Value;
        var dir = Path.GetDirectoryName(filePath);

        if (string.IsNullOrEmpty(dir))
        {
            logger.Warn<AppStateIoService>($"Invalid save path: '{filePath}'.");
            return;
        }

        try
        {
            Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(value, JsonSettings);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            logger.Error<AppStateIoService>($"Save failed for '{filePath}'.", ex);
            throw;
        }
    }

    public async Task SaveAsync(IAppState value, AbsolutePath path)
    {
        ArgumentNullException.ThrowIfNull(value);

        var filePath = path.Value;
        var dir = Path.GetDirectoryName(filePath);

        if (string.IsNullOrEmpty(dir))
        {
            logger.Warn<AppStateIoService>($"Invalid save path: '{filePath}'.");
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
            logger.Error<AppStateIoService>($"SaveAsync failed for '{filePath}'.", ex);
            throw;
        }
    }

    public IAppState? Load() => Load(GetAppStateFilePath());
    public Task<IAppState?> LoadAsync() => LoadAsync(GetAppStateFilePath());
    public void Save(IAppState state) => Save(state, GetAppStateFilePath());
    public Task SaveAsync(IAppState state) => SaveAsync(state, GetAppStateFilePath());
    
    private static AbsolutePath GetAppStateFilePath()
    {
        var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return new AbsolutePath(UnixPath.Combine(baseDir, AppName, FileName));
    }

    private const string AppName  = "Warkbench";
    private const string FileName = "appstate.json";
}
