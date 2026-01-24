using warkbench.src.basis.interfaces.Common;

namespace warkbench.src.basis.core.Common;

public sealed class ConsoleLogger : ILogger
{
    public void Info<TModule>(string message)
        => WriteLog<TModule>("INFO", message, ConsoleColor.Gray);

    public void Warn<TModule>(string message)
        => WriteLog<TModule>("WARN", message, ConsoleColor.Yellow);

    public void Error<TModule>(string message, Exception? ex = null)
    {
        WriteLog<TModule>("ERROR", message, ConsoleColor.Red);

        if (ex is null)
            return;

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"[EXCEPTION] {ex.Message}");
        Console.WriteLine($"[STACKTRACE] {ex.StackTrace}");
        Console.ResetColor();
    }

    private static void WriteLog<TModule>(
        string level,
        string message,
        ConsoleColor color)
    {
        var module = typeof(TModule).Name;

        Console.ForegroundColor = color;
        Console.WriteLine(
            $"[{DateTime.Now:HH:mm:ss}] [{level}] [{module}] {message}"
        );
        Console.ResetColor();
    }
}