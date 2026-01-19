using System;

namespace warkbench.Interop;

public interface IPlatformLibrary
{
    IntPtr GetFunction(IntPtr library, string symbol);
    string? GetLastErrorMessage();
    IntPtr LoadLib(string path);
    void FreeLib(IntPtr library);
}