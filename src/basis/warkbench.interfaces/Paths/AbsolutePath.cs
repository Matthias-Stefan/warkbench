namespace warkbench.src.basis.interfaces.Paths;

/// <summary>Represents a normalized, absolute filesystem path.</summary>
public readonly record struct AbsolutePath(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;
}