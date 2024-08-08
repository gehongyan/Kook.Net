using System.Text;

namespace Kook;

/// <summary>
///     表示一个用于记录日志的消息对象。
/// </summary>
public readonly struct LogMessage
{
    /// <summary>
    ///     获取日志记录的严重性。
    /// </summary>
    public LogSeverity Severity { get; }

    /// <summary>
    ///     获取日志记录的来源。
    /// </summary>
    public string Source { get; }

    /// <summary>
    ///     获取日志记录的消息。
    /// </summary>
    public string? Message { get; }

    /// <summary>
    ///     获取此日志条目的异常。
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    ///     使用事件的严重性、来源、消息和可选的异常初始化 <see cref="LogMessage"/> 结构。
    /// </summary>
    /// <param name="severity"> 事件的严重性。 </param>
    /// <param name="source"> 事件的来源。 </param>
    /// <param name="message"> 事件的消息。 </param>
    /// <param name="exception"> 事件的异常。 </param>
    public LogMessage(LogSeverity severity, string source, string? message, Exception? exception = null)
    {
        Severity = severity;
        Source = source;
        Message = message;
        Exception = exception;
    }

    /// <summary>
    ///     返回此日志消息的字符串表示形式。
    /// </summary>
    /// <returns> 此日志消息的字符串表示形式。 </returns>
    public override string ToString() => ToString();

    /// <summary>
    ///     返回此日志消息的字符串表示形式。
    /// </summary>
    /// <param name="builder"> 要使用的字符串构建器。 </param>
    /// <param name="fullException"> 是否在字符串中包含完整的异常信息。 </param>
    /// <param name="prependTimestamp"> 是否在字符串前添加时间戳。 </param>
    /// <param name="timestampKind"> 要使用的时间戳类型。 </param>
    /// <param name="padSource"> 源字符串的填充量。 </param>
    /// <returns> 此日志消息的字符串表示形式。 </returns>
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
