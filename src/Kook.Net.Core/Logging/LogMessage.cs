using System.Text;

namespace Kook;

/// <summary>
///     Provides a message object used for logging purposes.
/// </summary>
public readonly struct LogMessage
{
    /// <summary>
    ///     Gets the severity of the log entry.
    /// </summary>
    /// <returns>
    ///     A <see cref="LogSeverity"/> enum to indicate the severeness of the incident or event.
    /// </returns>
    public LogSeverity Severity { get; }

    /// <summary>
    ///     Gets the source of the log entry.
    /// </summary>
    /// <returns>
    ///     A string representing the source of the log entry.
    /// </returns>
    public string Source { get; }

    /// <summary>
    ///     Gets the message of this log entry.
    /// </summary>
    /// <returns>
    ///     A string containing the message of this log entry.
    /// </returns>
    public string? Message { get; }

    /// <summary>
    ///     Gets the exception of this log entry.
    /// </summary>
    /// <returns>
    ///     A <see cref="Kook.LogMessage.Exception" /> object associated with an incident; otherwise <c>null</c>.
    /// </returns>
    public Exception? Exception { get; }

    /// <summary>
    ///     Initializes a new <see cref="LogMessage"/> struct with the severity, source, message of the event, and
    ///     optionally, an exception.
    /// </summary>
    /// <param name="severity">The severity of the event.</param>
    /// <param name="source">The source of the event.</param>
    /// <param name="message">The message of the event.</param>
    /// <param name="exception">The exception of the event.</param>
    public LogMessage(LogSeverity severity, string source, string? message, Exception? exception = null)
    {
        Severity = severity;
        Source = source;
        Message = message;
        Exception = exception;
    }

    /// <summary>
    ///     Returns a string representation of this log message.
    /// </summary>
    /// <returns> A string representation of this log message. </returns>
    public override string ToString() => ToString();

    /// <summary>
    ///     Returns a string representation of this log message.
    /// </summary>
    /// <param name="builder"> The string builder to use. </param>
    /// <param name="fullException"> Whether to include the full exception in the string. </param>
    /// <param name="prependTimestamp"> Whether to prepend the timestamp to the string. </param>
    /// <param name="timestampKind"> The kind of timestamp to use. </param>
    /// <param name="padSource"> The amount of padding to use for the source. </param>
    /// <returns> A string representation of this log message. </returns>
    public string ToString(StringBuilder? builder = null, bool fullException = true, bool prependTimestamp = true,
        DateTimeKind timestampKind = DateTimeKind.Local, int? padSource = 11)
    {
        string? exMessage = fullException ? Exception?.ToString() : Exception?.Message;

        int maxLength =
            1
            + (prependTimestamp ? 8 : 0)
            + 1
            + (padSource ?? (Source?.Length ?? 0))
            + 1
            + (Message?.Length ?? 0)
            + (exMessage?.Length ?? 0)
            + 3;

        if (builder == null)
            builder = new StringBuilder(maxLength);
        else
        {
            builder.Clear();
            builder.EnsureCapacity(maxLength);
        }

        if (prependTimestamp)
        {
            DateTime now = timestampKind == DateTimeKind.Utc
                ? DateTime.UtcNow
                : DateTime.Now;

            builder.Append(now.ToString("HH:mm:ss"));
            builder.Append(' ');
        }

        if (Source != null)
        {
            if (padSource.HasValue)
            {
                if (Source.Length < padSource.Value)
                {
                    builder.Append(Source);
                    builder.Append(' ', padSource.Value - Source.Length);
                }
                else if (Source.Length > padSource.Value)
                    builder.Append(Source[..padSource.Value]);
                else
                    builder.Append(Source);
            }

            builder.Append(' ');
        }

        if (Message != null && !string.IsNullOrEmpty(Message))
        {
            foreach (char c in Message.Where(c => !char.IsControl(c)))
                builder.Append(c);
        }

        if (exMessage != null)
        {
            if (!string.IsNullOrEmpty(Message))
            {
                builder.Append(':');
                builder.AppendLine();
            }

            builder.Append(exMessage);
        }

        return builder.ToString();
    }
}
