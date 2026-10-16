using System.Diagnostics;

namespace Kook;

/// <summary>
///     倒计时模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record CountdownModule : IModule
{
    internal CountdownModule(CountdownMode mode, DateTimeOffset endTime, DateTimeOffset? startTime = null)
    {
        Mode = mode;
        EndTime = endTime;
        StartTime = startTime;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Countdown;

    /// <summary>
    ///     获取倒计时的结束时间。
    /// </summary>
    public DateTimeOffset EndTime { get; }

    /// <summary>
    ///     获取倒计时的开始时间。
    /// </summary>
    public DateTimeOffset? StartTime { get; }

    /// <summary>
    ///     获取倒计时的显示模式。
    /// </summary>
    public CountdownMode Mode { get; }

    private string DebuggerDisplay =>
        $"{Type}: To {EndTime:yyyy'/'M'/'d HH:mm:ss z} ({Mode} Mode{(StartTime is null ? string.Empty : $", From {EndTime:yyyy'/'M'/'d HH:mm:ss z}")})";
}
