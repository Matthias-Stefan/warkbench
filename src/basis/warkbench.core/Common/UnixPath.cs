using System.Diagnostics.CodeAnalysis;

namespace warkbench.src.basis.core.Common;

/// <summary> Provides utility methods for path manipulation enforced in Unix-style format (using forward slashes). </summary>
public static class UnixPath
{
    /// <summary> Combines segments into a normalized path using forward slashes while preserving absolute root indicators. </summary>
    public static string Combine(params string[] parts)
    {
        if (parts == null || parts.Length == 0)
            return string.Empty;

        var isAbsolute = parts[0].StartsWith("/");

        var combined = string.Join("/", parts
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select((p, i) => i == 0 ? p.TrimEnd('/') : p.Trim('/')));

        return isAbsolute ? "/" + combined.TrimStart('/') : combined;
    }

    /// <summary> Returns the directory portion of a path string, normalized with forward slashes. </summary>
    public static string GetDirectoryName(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        var dir = Path.GetDirectoryName(path);
        return string.IsNullOrEmpty(dir) ? string.Empty : dir.Replace('\\', '/').TrimEnd('/');
    }

    /// <summary> Extracts the file name and extension from a path string in a cross-platform compliant format. </summary>
    [return: NotNullIfNotNull(nameof(path))]
    public static string? GetFileName(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        var file = Path.GetFileName(path);
        return ToUnix(file);
    }

    /// <summary> Resolves the fully qualified absolute path and normalizes all separators to forward slashes. </summary>
    [return: NotNullIfNotNull(nameof(path))]
    public static string? GetFullPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        var full = Path.GetFullPath(path);
        return ToUnix(full);
    }
    
    /// <summary> Normalizes all backslashes in a given path string to forward slashes. </summary>
    [return: NotNullIfNotNull(nameof(path))]
    public static string? ToUnix(string? path)
    {
        return path?.Replace('\\', '/');
    }
}