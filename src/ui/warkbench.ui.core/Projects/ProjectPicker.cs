using Avalonia.Platform.Storage;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.ui.core.Window;

namespace warkbench.src.ui.core.Projects;

public sealed class ProjectPicker(
    MainWindowProvider mainWindowProvider,
    IPathService pathService) : IProjectPicker
{
    public async Task<string?> Open()
    {
        var mainWindow = mainWindowProvider.Get!;
        var storageProvider = mainWindow.StorageProvider;
        var projectsPath = pathService.ProjectsPath.Value;
        IStorageFolder? startFolder = null;
        if (!string.IsNullOrWhiteSpace(projectsPath))
        {
            startFolder = await storageProvider.TryGetFolderFromPathAsync(projectsPath);
        }
        
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Project",
            AllowMultiple = false,
            SuggestedStartLocation = startFolder,
            FileTypeFilter =
            [
                new FilePickerFileType("Warkbench Project")
                {
                    Patterns = [$"*{IProjectIoService.Extension}"]
                }
            ]
        });

        return files.Count == 0 ? string.Empty : files[0].TryGetLocalPath();
    }
}