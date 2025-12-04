using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace warkbench.Interop;

public static class BitmapHelper
{
    public static WriteableBitmap CreateFromRgba(List<byte> data, int width, int height)
    {
        if (data.Count != width * height * 4)
        { 
            throw new ArgumentException("Unexpected data size for RGBA8888 format.");
        }

        var bitmap = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Rgba8888,
            AlphaFormat.Unpremul);

        using (var fb = bitmap.Lock())
        {
            Marshal.Copy(data.ToArray(), 0, fb.Address, data.Count);
        }

        return bitmap;
    }
}
