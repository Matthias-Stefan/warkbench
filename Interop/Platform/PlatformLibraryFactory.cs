using System.Runtime.InteropServices;

namespace warkbench.Interop;

public static class PlatformLibraryFactory
{
    public static IPlatformLibrary? GetPlatformLibrary()
    {
        IPlatformLibrary? platformLibrary = null;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            platformLibrary = new WindowsLibrary();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            platformLibrary = new UnixLibrary();
        }
        return platformLibrary;
    }
}