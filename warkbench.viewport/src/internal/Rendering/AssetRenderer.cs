using Avalonia.Media;
using Avalonia;
using System.Collections.Generic;


namespace warkbench.viewport;
internal class AssetRenderer
{
    public void Render(DrawingContext ctx, ViewportCamera cam, Rect viewportBounds, IEnumerable<IRenderable> renderables)
    {
        using var _ = ctx.PushClip(viewportBounds);

        foreach (var renderable in renderables)
        {
            switch (renderable)
            {
                case LineRenderable line:
                {
                    var brush = new SolidColorBrush(line.Color)
                    {
                        Opacity = line.Opacity
                    };

                    var pen = new Pen(brush, line.Thickness)
                    {
                        LineCap = line.LineCap,
                        LineJoin = line.LineJoin
                    };

                    ctx.DrawLine(
                        pen,
                        line.StartPoint,
                        line.EndPoint
                    );
                    break;
                }

                case RectRenderable rect:
                {
                    IBrush? fillBrush = null;
                    if (rect.FillOpacity > 0.0f)
                    {
                        fillBrush = new SolidColorBrush(rect.FillColor)
                        {
                            Opacity = rect.FillOpacity
                        };
                    }

                    Pen? strokePen = null;
                    if (rect.HasStroke)
                    {
                        strokePen = new Pen(
                            new SolidColorBrush(rect.StrokeColor!.Value)
                            {
                                Opacity = rect.StrokeOpacity
                            },
                            rect.StrokeThickness
                        );
                    }

                    if (rect.CornerRadius > 0.0f)
                    {
                        ctx.DrawRectangle(
                            fillBrush,
                            strokePen,
                            rect.Bounds,
                            rect.CornerRadius,
                            rect.CornerRadius
                        );
                    }
                    else
                    {
                        ctx.DrawRectangle(
                            fillBrush,
                            strokePen,
                            rect.Bounds
                        );
                    }

                    break;
                }

                case SpriteRenderable sprite:
                {
                    var opacity = sprite.Opacity;
                    if (opacity <= 0.0f) 
                        break;

                    var bmp = sprite.Content;
                    var src = sprite.SourceRect ?? new Rect(0, 0, bmp.Size.Width, bmp.Size.Height);

                    if (src.Width <= 0 || src.Height <= 0 || sprite.Bounds.Width <= 0 || sprite.Bounds.Height <= 0)
                        break;

                    using (ctx.PushOpacity(opacity))
                    {
                        ctx.DrawImage(bmp, src, sprite.Bounds);

                        if (sprite.Tint is { } tint && tint.A > 0)
                        {
                            var maskBrush = new ImageBrush(bmp)
                            {
                                SourceRect = new RelativeRect(src, RelativeUnit.Absolute),
                                Stretch = Stretch.Fill
                            };

                            using (ctx.PushOpacityMask(maskBrush, sprite.Bounds))
                            {
                                ctx.FillRectangle(new SolidColorBrush(tint), sprite.Bounds);
                            }
                        }
                    }
                    break;
                }

                case TextRenderable text:
                {
                    if (text.Opacity <= 0.0f || string.IsNullOrEmpty(text.Text))
                        break;

                    var brush = new SolidColorBrush(text.Color)
                    {
                        Opacity = text.Opacity
                    };

                    var typeface = new Typeface(
                        new FontFamily(text.FontFamily),
                        text.FontStyle,
                        text.FontWeight
                    );

                    var formattedText = new FormattedText(
                        text.Text,
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        text.FontSize,
                        brush
                    );

                    ctx.DrawText(formattedText, text.Position);
                    break;
                }

                default:
                    // Optional: unknown renderable type
                    break;
            }
        }



        var size = viewportBounds.Size;
        
        
    }
}