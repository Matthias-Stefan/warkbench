namespace warkbench.src.basis.interfaces.Paths;

/// <summary>Represents a normalized, application-relative filesystem path.</summary>
public readonly record struct LocalPath(string Value)
{
    public override string ToString() => Value;
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
}
