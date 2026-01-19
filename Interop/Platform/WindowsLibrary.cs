using System;
using System.Runtime.InteropServices;

namespace warkbench.Interop;

public class WindowsLibrary : IPlatformLibrary
{
    private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

    public IntPtr GetFunction(IntPtr library, string symbol)
    { 
        return GetProcAddress(library, symbol);
    }

    public string? GetLastErrorMessage()
    {
        uint errCode = GetLastError();
        var sb = new System.Text.StringBuilder(256);
        FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, errCode, 0, sb, sb.Capacity, IntPtr.Zero);
        return sb.ToString();
    }

    public IntPtr LoadLib(string path)
    { 
        return LoadLibrary(string.Concat(path, ".dll"));
    }

    public void FreeLib(IntPtr library)
    {
        if (library != IntPtr.Zero)
        { 
            FreeLibrary(library);
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32.dll")]
    private static extern uint GetLastError();

    [DllImport("kernel32.dll")]
    private static extern int FormatMessage(
        uint dwFlags, IntPtr lpSource, uint dwMessageId,
        uint dwLanguageId, [Out] System.Text.StringBuilder lpBuffer,
        int nSize, IntPtr Arguments);
}