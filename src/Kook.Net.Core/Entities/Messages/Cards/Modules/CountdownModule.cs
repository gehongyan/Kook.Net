using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     倒计时模块，可用于 <see cref="ICard"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class CountdownModule : IModule, IEquatable<CountdownModule>, IEquatable<IModule>
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

    /// <summary>
    ///     判定两个 <see cref="CountdownModule"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="CountdownModule"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(CountdownModule left, CountdownModule right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="CountdownModule"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="CountdownModule"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(CountdownModule left, CountdownModule right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is CountdownModule countdownModule && Equals(countdownModule);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] CountdownModule? countdownModule) =>
        GetHashCode() == countdownModule?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode() =>
        (Type, EndTime, StartTime, Mode).GetHashCode();

    bool IEquatable<IModule>.Equals([NotNullWhen(true)] IModule? module) =>
        Equals(module as CountdownModule);
}
