using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     倒计时模块
/// </summary>
/// <remarks>
///     展示倒计时
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class CountdownModule : IModule
{
    public ModuleType Type => ModuleType.Countdown;

    internal CountdownModule(CountdownMode mode, DateTimeOffset endTime, DateTimeOffset? startTime = null)
    {
        Mode = mode;
        EndTime = endTime;
        StartTime = startTime;
    }
    
    public DateTimeOffset EndTime { get; internal set; }

    public DateTimeOffset? StartTime { get; internal set; }

    public CountdownMode Mode { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: To {EndTime:yyyy'/'M'/'d HH:mm:ss z} ({Mode} Mode{(StartTime is null? string.Empty: $", From {EndTime:yyyy'/'M'/'d HH:mm:ss z}")})";
}