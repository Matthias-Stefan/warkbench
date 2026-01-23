using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.interfaces.Io;

/// <summary>
/// Provides a unified interface for physical file system access and object persistence.
/// </summary>
public interface IIoService
{
    /// <summary> Serializes an object to a file at the given path. </summary>
    void Save<T>(T value, AbsolutePath path) where T : class;

    /// <summary> Reads a file and deserializes its content into a new object instance. </summary>
    T? Load<T>(AbsolutePath path) where T : class;
}