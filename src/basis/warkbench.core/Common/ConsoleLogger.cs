using warkbench.src.basis.interfaces.Common;

namespace warkbench.src.basis.core.Common;

public class ConsoleLogger : ILogger
{
    public void Info(string message) => WriteLog("INFO", message, ConsoleColor.Gray);

    public void Warn(string message) => WriteLog("WARN", message, ConsoleColor.Yellow);

    public void Error(string message, Exception? ex = null)
    {
        WriteLog("ERROR", message, ConsoleColor.Red);
        if (ex == null) 
            return;
        
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"[EXCEPTION] {ex.Message}");
        Console.WriteLine($"[STACKTRACE] {ex.StackTrace}");
        Console.ResetColor();
    }

    private static void WriteLog(string level, string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{level}] {message}");
        Console.ResetColor();
    }
}