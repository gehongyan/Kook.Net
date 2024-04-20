using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a countdown module builder for creating a <see cref="CountdownModule"/>.
/// </summary>
public class CountdownModuleBuilder : IModuleBuilder, IEquatable<CountdownModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Countdown;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CountdownModuleBuilder"/> class.
    /// </summary>
    public CountdownModuleBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CountdownModuleBuilder"/> class.
    /// </summary>
    public CountdownModuleBuilder(CountdownMode mode, DateTimeOffset endTime, DateTimeOffset? startTime = null)
    {
        Mode = mode;
        EndTime = endTime;
        StartTime = startTime;
    }

    /// <summary>
    ///  Gets or sets the ending time of the countdown.
    /// </summary>
    /// <returns>
    ///     The time at which the countdown ends.
    /// </returns>
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    ///     Gets or sets the beginning time of the countdown.
    /// </summary>
    /// <returns>
    ///     The time at which the countdown begins.
    /// </returns>
    public DateTimeOffset? StartTime { get; set; }

    /// <summary>
    ///     Gets or sets how the countdown should be displayed.
    /// </summary>
    /// <returns>
    ///     A <see cref="CountdownMode"/> representing how the countdown should be displayed.
    /// </returns>
    public CountdownMode Mode { get; set; }

    /// <summary>
    ///     Sets how the countdown should be displayed.
    /// </summary>
    /// <param name="mode">
    ///     A <see cref="CountdownMode"/> representing how the countdown should be displayed.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CountdownModuleBuilder WithMode(CountdownMode mode)
    {
        Mode = mode;
        return this;
    }

    /// <summary>
    ///     Sets the beginning time of the countdown.
    /// </summary>
    /// <param name="endTime">
    ///     The time at which the countdown ends.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CountdownModuleBuilder WithEndTime(DateTimeOffset endTime)
    {
        EndTime = endTime;
        return this;
    }

    /// <summary>
    ///     Sets the beginning time of the countdown.
    /// </summary>
    /// <param name="startTime">
    ///     The time at which the countdown begins.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CountdownModuleBuilder WithStartTime(DateTimeOffset? startTime)
    {
        StartTime = startTime;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="CountdownModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="CountdownModule"/> representing the built countdown module object.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="CountdownMode"/> is not <see cref="CountdownMode.Second"/> but <see cref="StartTime"/> is set.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="EndTime"/> is before the current time.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="StartTime"/> is before the Unix epoch.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="EndTime"/> is equal or before <see cref="StartTime"/>
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
    ///     Determines whether the specified <see cref="CountdownModuleBuilder"/> is equal to the current <see cref="CountdownModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="CountdownModuleBuilder"/> is equal to the current <see cref="CountdownModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(CountdownModuleBuilder? left, CountdownModuleBuilder? right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="CountdownModuleBuilder"/> is not equal to the current <see cref="CountdownModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="CountdownModuleBuilder"/> is not equal to the current <see cref="CountdownModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(CountdownModuleBuilder? left, CountdownModuleBuilder? right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="CountdownModuleBuilder"/> is equal to the current <see cref="CountdownModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="CountdownModuleBuilder"/>, <see cref="Equals(CountdownModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="CountdownModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="CountdownModuleBuilder"/> is equal to the current <see cref="CountdownModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is CountdownModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="CountdownModuleBuilder"/> is equal to the current <see cref="CountdownModuleBuilder"/>.</summary>
    /// <param name="countdownModuleBuilder">The <see cref="CountdownModuleBuilder"/> to compare with the current <see cref="CountdownModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="CountdownModuleBuilder"/> is equal to the current <see cref="CountdownModuleBuilder"/>; otherwise, <c>false</c>.</returns>
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
