namespace warkbench.src.basis.interfaces.Common;

/// <summary>
/// Provides a standardized interface for diagnostic messaging and error tracking.
/// </summary>
public interface ILogger
{
    /// <summary>Logs non-critical operational messages for tracing application flow.</summary>
    void Info<TModule>(string message);

    /// <summary>Logs non-terminal issues that require attention but do not halt execution.</summary>
    void Warn<TModule>(string message);

    /// <summary>Logs critical failures and optional exception metadata for debugging.</summary>
    void Error<TModule>(string message, Exception? ex = null);
}