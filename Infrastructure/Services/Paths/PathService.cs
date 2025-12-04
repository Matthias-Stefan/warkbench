using System;


namespace warkbench.Infrastructure;
public class PathService
{
    private string BasePath { get; init; }
    public string ToolsPath => UnixPath.GetFullPath(UnixPath.Combine(BasePath, "tools"));
    public string DataPath => UnixPath.GetFullPath(UnixPath.Combine(BasePath, "data"));
    public string SrcPath => UnixPath.GetFullPath(UnixPath.Combine(BasePath, "src"));
    public string AssetsPath => UnixPath.GetFullPath(UnixPath.Combine(BasePath, "assets"));

    public PathService()
    {
        var exePath = AppContext.BaseDirectory;
        // TODO: config
        BasePath = UnixPath.GetFullPath(UnixPath.Combine(exePath, "../../../../warpunk.emberfall/"));
    }
}
