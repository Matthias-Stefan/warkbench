using Avalonia;

namespace warkbench.src.ui.viewport.Rendering;

public interface IRenderable
{
    int Layer { get; }
    Rect Bounds { get; }
}