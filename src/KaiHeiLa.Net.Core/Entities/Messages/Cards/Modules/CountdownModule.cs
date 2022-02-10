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
        // if (mode == CountdownMode.Second && startTime >= endTime)
        // {
        //     throw new InvalidOperationException($"{StartTime} 应早于 {EndTime}");
        // }
        // if (mode != CountdownMode.Second && startTime is not null)
        // {
        //     throw new InvalidOperationException($"仅当 {Mode} 为 {CountdownMode.Second} 才有 {nameof(StartTime)} 字段");
        // }
        
        Mode = mode;
        EndTime = endTime;
        StartTime = startTime;
    }
    
    public DateTimeOffset EndTime { get; internal set; }
    // {
    //     get => _endTime;
    //     internal set
    //     {
    //         if (value <= DateTimeOffset.Now)
    //         {
    //             throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(EndTime)} 不能小于服务器当前时间戳");
    //         }
    //         _endTime = value;
    //     }
    // }

    public DateTimeOffset? StartTime { get; internal set; }
    // {
    //     get => _startTime;
    //     internal set
    //     {
    //         if (value is null)
    //         {
    //             _startTime = value;
    //             return;
    //         }
    //         if (value <= DateTimeOffset.Now)
    //         {
    //             throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(StartTime)} 不能小于服务器当前时间戳");
    //         }
    //         _startTime = value;
    //     }
    // }

    public CountdownMode Mode { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: To {EndTime:yyyy'/'M'/'d HH:mm:ss z} ({Mode} Mode{(StartTime is null? string.Empty: $", From {EndTime:yyyy'/'M'/'d HH:mm:ss z}")})";
}