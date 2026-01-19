using System;
using System.Runtime.InteropServices;


namespace warkbench.Interop;

[StructLayout(LayoutKind.Sequential)]
public struct Image
{
    public uint width;
    public uint height;
    public uint format;
    public ulong data_size;
    public IntPtr data;
}

public partial class ImageLoader
{
    private readonly LoadImageDelegate _loadImageDelegate;
    private readonly FreeImageDelegate _freeImageDelegate;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool LoadImageDelegate([MarshalAs(UnmanagedType.LPStr)] string path, out Image image);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool FreeImageDelegate(ref Image image);

    public object? Load(string path)
    {
        if (!_loadImageDelegate(path, out var image))
        {
            return null;
        }
        
        return new object();
    }

    [LibraryImport("stb_image")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.I1)]
    private static partial bool load_image([MarshalAs(UnmanagedType.LPStr)] string path, out Image image);

    [LibraryImport("stb_image")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.I1)]
    private static partial void free_image(ref Image image);

    public ImageLoader(IPlatformLibrary platform)
    {
        var lib = platform.LoadLib("stb_image");
        if (lib == IntPtr.Zero)
        {
            Console.WriteLine($"LoadLib failed: {platform.GetLastErrorMessage()}");
        }

        var loadImageProc = platform.GetFunction(lib, "load_image");
        if (loadImageProc == IntPtr.Zero)
        {
            Console.WriteLine($"GetFunction load_image failed: {platform.GetLastErrorMessage()}");
        }

        var freeImageProc = platform.GetFunction(lib, "free_image");
        if (freeImageProc == IntPtr.Zero)
        {
            Console.WriteLine($"GetFunction free_image failed: {platform.GetLastErrorMessage()}");
        }

        _loadImageDelegate = Marshal.GetDelegateForFunctionPointer<LoadImageDelegate>(loadImageProc);
        _freeImageDelegate = Marshal.GetDelegateForFunctionPointer<FreeImageDelegate>(freeImageProc);
    }
}