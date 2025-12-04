using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using warkbench.Infrastructure;
using warkbench.Models;

namespace warkbench.Interop;

[StructLayout(LayoutKind.Sequential)]
public struct ObjMesh
{
    public uint vertex_count;
    public uint index_count;
    public IntPtr vertices;
    public IntPtr indices;
}

public partial class ObjLoader 
{
    private readonly LoadObjDelegate _loadObjDelegate;
    private readonly FreeObjDelegate _freeObjDelegate;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool LoadObjDelegate([MarshalAs(UnmanagedType.LPStr)] string path, out ObjMesh mesh);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool FreeObjDelegate(ref ObjMesh mesh);

    public object? Load(string path)
    {
        if (!_loadObjDelegate(path, out var mesh))
        {
            return null;
        }
        
        return new object();
    }

    [LibraryImport("tiny_obj_loader")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.I1)]
    private static partial bool load_obj([MarshalAs(UnmanagedType.LPStr)] string path, out ObjMesh mesh);

    [LibraryImport("tiny_obj_loader")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.I1)]
    private static partial void free_obj_mesh(ref ObjMesh mesh);

    public ObjLoader(IPlatformLibrary platform)
    {
        var lib = platform.LoadLib("tiny_obj_loader");
        if (lib == IntPtr.Zero)
        {
            Console.WriteLine($"LoadLib failed: {platform.GetLastErrorMessage()}");
        }

        var loadObjProc = platform.GetFunction(lib, "load_obj");
        if (loadObjProc == IntPtr.Zero)
        {
            Console.WriteLine($"GetFunction load_obj failed: {platform.GetLastErrorMessage()}");
        }

        var freeObjMeshProc = platform.GetFunction(lib, "free_obj_mesh");
        if (freeObjMeshProc == IntPtr.Zero)
        {
            Console.WriteLine($"GetFunction free_obj_mesh failed: {platform.GetLastErrorMessage()}");
        }

        _loadObjDelegate = Marshal.GetDelegateForFunctionPointer<LoadObjDelegate>(loadObjProc);
        _freeObjDelegate = Marshal.GetDelegateForFunctionPointer<FreeObjDelegate>(freeObjMeshProc);
    }
}
