using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace warkbench.Infrastructure;

public static class UnixPath
{
    public static string Combine(params string[] parts)
    {
        if (parts == null || parts.Length == 0)
        { 
            return string.Empty;
        }

        bool isAbsolute = parts[0].StartsWith("/");

        var combined = string.Join("/", parts
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select((p, i) => i == 0 ? p.TrimEnd('/') : p.Trim('/')));

        return isAbsolute ? "/" + combined.TrimStart('/') : combined;
    }

    public static string GetDirectoryName(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        string? dir = Path.GetDirectoryName(path);

        if (string.IsNullOrEmpty(dir))
        {
            return string.Empty;
        }

        return dir.Replace('\\', '/').TrimEnd('/');
    }

    [return: NotNullIfNotNull(nameof(path))]
    public static string? GetFileName(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        string file = Path.GetFileName(path);

        return file.Replace('\\', '/');
    }

    [return: NotNullIfNotNull(nameof(path))]
    public static string? GetFullPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        string full = Path.GetFullPath(path);

        return full.Replace('\\', '/');
    }
}
