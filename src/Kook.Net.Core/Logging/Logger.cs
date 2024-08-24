namespace Kook.Logging;

internal class Logger
{
    private readonly LogManager _manager;

    public string Name { get; }

    public LogSeverity Level => _manager.Level;

    public Logger(LogManager manager, string name)
    {
        _manager = manager;
        Name = name;
    }

    public Task LogAsync(LogSeverity severity, Exception? exception = null) =>
        _manager.LogAsync(severity, Name, exception);

    public Task LogAsync(LogSeverity severity, string message, Exception? exception = null) =>
        _manager.LogAsync(severity, Name, message, exception);

    public Task LogAsync(LogSeverity severity, FormattableString message, Exception? exception = null) =>
        _manager.LogAsync(severity, Name, message, exception);

    public Task ErrorAsync(Exception? exception) => _manager.ErrorAsync(Name, exception);

    public Task ErrorAsync(string message, Exception? exception = null) =>
        _manager.ErrorAsync(Name, message, exception);

    public Task ErrorAsync(FormattableString message, Exception? exception = null) =>
        _manager.ErrorAsync(Name, message, exception);

    public Task WarningAsync(Exception? exception) => _manager.WarningAsync(Name, exception);

    public Task WarningAsync(string message, Exception? exception = null) =>
        _manager.WarningAsync(Name, message, exception);

    public Task WarningAsync(FormattableString message, Exception? exception = null) =>
        _manager.WarningAsync(Name, message, exception);

    public Task InfoAsync(Exception? exception) => _manager.InfoAsync(Name, exception);

    public Task InfoAsync(string message, Exception? exception = null) => _manager.InfoAsync(Name, message, exception);

    public Task InfoAsync(FormattableString message, Exception? exception = null) =>
        _manager.InfoAsync(Name, message, exception);

    public Task VerboseAsync(Exception? exception) => _manager.VerboseAsync(Name, exception);

    public Task VerboseAsync(string message, Exception? exception = null) =>
        _manager.VerboseAsync(Name, message, exception);

    public Task VerboseAsync(FormattableString message, Exception? exception = null) =>
        _manager.VerboseAsync(Name, message, exception);

    public Task DebugAsync(Exception? exception) => _manager.DebugAsync(Name, exception);

    public Task DebugAsync(string message, Exception? exception = null) =>
        _manager.DebugAsync(Name, message, exception);

    public Task DebugAsync(FormattableString message, Exception? exception = null) =>
        _manager.DebugAsync(Name, message, exception);
}
