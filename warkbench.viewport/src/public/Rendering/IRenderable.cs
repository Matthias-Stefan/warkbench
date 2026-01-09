using Avalonia;

namespace warkbench.viewport;
public interface IRenderable
{
    int Layer { get; }
    Rect Bounds { get; }
}