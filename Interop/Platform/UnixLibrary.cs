using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace warkbench.Interop;

public class UnixLibrary : IPlatformLibrary
{
    private const int RTLD_NOW = 2;

    public IntPtr GetFunction(IntPtr library, string symbol)
    {
        var symbolPtr = dlsym(library, symbol);
        if (symbolPtr == IntPtr.Zero)
        {
            var errorPtr = dlerror();
            string? msg = Marshal.PtrToStringAnsi(errorPtr);
            throw new InvalidOperationException($"dlsym failed: {msg}");
        }
        return symbolPtr;
    }

    public string? GetLastErrorMessage()
    {
        IntPtr errPtr = dlerror();
        return errPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errPtr) : null;
    }

    public IntPtr LoadLib(string path)
    {
        IntPtr handle = IntPtr.Zero;
        if (path == "tiny_obj_loader")
        {
            handle = dlopen(string.Concat("/home/ms/Documents/warpunk/warpunk.tools/warkbench/3rdParty//tiny_obj_loader.so"), RTLD_NOW);
        }
        else if (path == "stb_image")
        { 
            handle = dlopen(string.Concat("/home/ms/Documents/warpunk/warpunk.tools/warkbench/3rdParty//stb_image.so"), RTLD_NOW);
        }

        if (handle == IntPtr.Zero)
        {
            var errorPtr = dlerror();
            string? msg = Marshal.PtrToStringAnsi(errorPtr);
            throw new InvalidOperationException($"dlopen failed: {msg}");
        }
        return handle;
    }

    public void FreeLib(IntPtr library)
    {
        if (library != IntPtr.Zero)
        {
            dlclose(library);
        }
    }

    [DllImport("libdl.so")]
    private static extern IntPtr dlopen(string fileName, int flags);

    [DllImport("libdl.so")]
    private static extern IntPtr dlsym(IntPtr handle, string symbol);

    [DllImport("libdl.so")]
    private static extern int dlclose(IntPtr handle);

    [DllImport("libdl.so")]
    private static extern IntPtr dlerror();
}