using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents a countdown module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class CountdownModule : IModule
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
    ///     Gets the end time of the countdown.
    /// </summary>
    /// <returns>
    ///     The end time of the countdown.
    /// </returns>
    public DateTimeOffset EndTime { get; internal set; }

    /// <summary>
    ///     Gets the start time of the countdown.
    /// </summary>
    /// <returns>
    ///     The start time of the countdown.
    /// </returns>
    public DateTimeOffset? StartTime { get; internal set; }

    /// <summary>
    ///     Gets the mode of the countdown.
    /// </summary>
    /// <returns>
    ///     A <see cref="CountdownMode"/> value that represents the mode of the countdown.
    /// </returns>
    public CountdownMode Mode { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: To {EndTime:yyyy'/'M'/'d HH:mm:ss z} ({Mode} Mode{(StartTime is null? string.Empty: $", From {EndTime:yyyy'/'M'/'d HH:mm:ss z}")})";
}