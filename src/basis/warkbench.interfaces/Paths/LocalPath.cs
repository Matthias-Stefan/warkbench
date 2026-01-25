namespace warkbench.src.basis.interfaces.Paths;

/// <summary>Represents a normalized, application-relative filesystem path.</summary>
public readonly record struct LocalPath(string Value)
{
    /// <summary>Gets the parent directory of this path.</summary>
    public LocalPath Parent()
    {
        if (IsEmpty)
            return default;

        var parent = Path.GetDirectoryName(Value.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        return string.IsNullOrEmpty(parent)
            ? default
            : new LocalPath(parent);
    }
    
    public override string ToString() => Value;
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
}
