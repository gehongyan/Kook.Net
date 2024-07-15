using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="CountdownModule"/> 模块的构建器。
/// </summary>
public class CountdownModuleBuilder : IModuleBuilder, IEquatable<CountdownModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Countdown;

    /// <summary>
    ///     初始化一个 <see cref="CountdownModuleBuilder"/> 类的新实例。
    /// </summary>
    public CountdownModuleBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="CountdownModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="mode"> 倒计时的显示模式。 </param>
    /// <param name="endTime"> 倒计时结束的时间。 </param>
    /// <param name="startTime"> 倒计时开始的时间。 </param>
    public CountdownModuleBuilder(CountdownMode mode, DateTimeOffset endTime, DateTimeOffset? startTime = null)
    {
        Mode = mode;
        EndTime = endTime;
        StartTime = startTime;
    }

    /// <summary>
    ///     获取或设置倒计时结束的时间。
    /// </summary>
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    ///     获取或设置倒计时开始的时间。
    /// </summary>
    /// <remarks>
    ///     仅当 <see cref="Mode"/> 为 <see cref="F:Kook.CountdownMode.Second"/> 时，才允许设置 <see cref="StartTime"/>。
    /// </remarks>
    public DateTimeOffset? StartTime { get; set; }

    /// <summary>
    ///     获取或设置倒计时的显示模式。
    /// </summary>
    public CountdownMode Mode { get; set; }

    /// <summary>
    ///     设置倒计时的显示模式。
    /// </summary>
    /// <param name="mode"> 倒计时的显示模式。 </param>
    /// <returns> 当前构建器。 </returns>
    public CountdownModuleBuilder WithMode(CountdownMode mode)
    {
        Mode = mode;
        return this;
    }

    /// <summary>
    ///     设置倒计时结束的时间。
    /// </summary>
    /// <param name="endTime"> 倒计时结束的时间。 </param>
    /// <returns> 当前构建器。 </returns>
    public CountdownModuleBuilder WithEndTime(DateTimeOffset endTime)
    {
        EndTime = endTime;
        return this;
    }

    /// <summary>
    ///     设置倒计时开始的时间。
    /// </summary>
    /// <param name="startTime"> 倒计时开始的时间。 </param>
    /// <returns> 当前构建器。 </returns>
    /// <remarks>
    ///     仅当 <see cref="Mode"/> 为 <see cref="F:Kook.CountdownMode.Second"/> 时，才允许设置 <see cref="StartTime"/>。
    /// </remarks>
    public CountdownModuleBuilder WithStartTime(DateTimeOffset? startTime)
    {
        StartTime = startTime;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="CountdownModule"/> 对象。
    /// </summary>
    /// <returns>
    ///     由当前构建器表示的属性构建的 <see cref="CountdownModule"/> 对象。
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="CountdownMode"/> 不为 <see cref="F:Kook.CountdownMode.Second"/> 时，不允许设置 <see cref="StartTime"/>。
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="EndTime"/> 早于当前时间。
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="StartTime"/> 早于 Unix 纪元时间。
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="EndTime"/> 应晚于 <see cref="StartTime"/>。
    /// </exception>
    public CountdownModule Build()
    {
        if (Mode != CountdownMode.Second && StartTime is not null)
            throw new InvalidOperationException(
                "Only when the countdown is displayed as second mode can the start time be set.");

        if (EndTime < DateTimeOffset.Now)
            throw new ArgumentOutOfRangeException(
                nameof(EndTime),
                $"{nameof(EndTime)} must be equal or later than current timestamp.");

        if (StartTime is not null && StartTime < DateTimeOffset.FromUnixTimeSeconds(0))
            throw new ArgumentOutOfRangeException(
                message: $"{nameof(StartTime)} must be equal or later than Unix epoch.",
                paramName: nameof(StartTime));

        if (StartTime >= EndTime)
            throw new ArgumentOutOfRangeException(
                message: $"{nameof(StartTime)} must be later than {nameof(EndTime)}.",
                paramName: nameof(StartTime));

        return new CountdownModule(Mode, EndTime, StartTime);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="CountdownModuleBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="CountdownModuleBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(CountdownModuleBuilder? left, CountdownModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="CountdownModuleBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="CountdownModuleBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(CountdownModuleBuilder? left, CountdownModuleBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is CountdownModuleBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] CountdownModuleBuilder? countdownModuleBuilder)
    {
        if (countdownModuleBuilder is null) return false;

        return Type == countdownModuleBuilder.Type
            && EndTime == countdownModuleBuilder.EndTime
            && StartTime == countdownModuleBuilder.StartTime
            && Mode == countdownModuleBuilder.Mode;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as CountdownModuleBuilder);
}
