using System.Diagnostics.CodeAnalysis;
using warkbench.Models;


namespace warkbench.Infrastructure;

public interface IOBase
{
    void Save(object? value, string? path);
    public T? Load<T>(string? path) where T : class;
}