using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a countdown module that can be used in an <see cref="ICard"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class CountdownModule : IModule, IEquatable<CountdownModule>
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
    public DateTimeOffset EndTime { get; }

    /// <summary>
    ///     Gets the start time of the countdown.
    /// </summary>
    /// <returns>
    ///     The start time of the countdown.
    /// </returns>
    public DateTimeOffset? StartTime { get; }

    /// <summary>
    ///     Gets the mode of the countdown.
    /// </summary>
    /// <returns>
    ///     A <see cref="CountdownMode"/> value that represents the mode of the countdown.
    /// </returns>
    public CountdownMode Mode { get; }
    
    private string DebuggerDisplay => $"{Type}: To {EndTime:yyyy'/'M'/'d HH:mm:ss z} ({Mode} Mode{(StartTime is null? string.Empty: $", From {EndTime:yyyy'/'M'/'d HH:mm:ss z}")})";
    
    public static bool operator ==(CountdownModule left, CountdownModule right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(CountdownModule left, CountdownModule right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="CountdownModule"/> is equal to the current <see cref="CountdownModule"/>.</summary>
    /// <remarks>If the object passes is an <see cref="CountdownModule"/>, <see cref="Equals(CountdownModule)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="CountdownModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="CountdownModule"/> is equal to the current <see cref="CountdownModule"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is CountdownModule countdownModule && Equals(countdownModule);

    /// <summary>Determines whether the specified <see cref="CountdownModule"/> is equal to the current <see cref="CountdownModule"/>.</summary>
    /// <param name="countdownModule">The <see cref="CountdownModule"/> to compare with the current <see cref="CountdownModule"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="CountdownModule"/> is equal to the current <see cref="CountdownModule"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(CountdownModule countdownModule)
        => GetHashCode() == countdownModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Type, EndTime, StartTime, Mode).GetHashCode();
}