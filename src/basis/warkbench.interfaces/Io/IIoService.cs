using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.interfaces.Io;

/// <summary>Provides file system access and persistence for a specific model type.</summary>
public interface IIoService<T> where T : class
{
    /// <summary>Reads a file and deserializes its content into a new instance.</summary>
    T? Load(AbsolutePath path);

    /// <summary>Asynchronously reads a file and deserializes its content into a new instance.</summary>
    Task<T?> LoadAsync(AbsolutePath path);
    
    /// <summary>Serializes a value to a file at the given path.</summary>
    void Save(T value, AbsolutePath path);

    /// <summary>Asynchronously serializes a value to a file at the given path.</summary>
    Task SaveAsync(T value, AbsolutePath path);
}