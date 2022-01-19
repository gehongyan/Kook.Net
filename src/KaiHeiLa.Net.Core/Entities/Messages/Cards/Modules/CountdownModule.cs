namespace KaiHeiLa;

/// <summary>
///     倒计时模块
/// </summary>
/// <remarks>
///     展示倒计时
/// </remarks>
public class CountdownModule : IModule
{
    private DateTimeOffset _endTime;
    private DateTimeOffset? _startTime;
    public ModuleType Type => ModuleType.Countdown;

    public CountdownModule(CountdownMode mode, DateTimeOffset endTime, DateTimeOffset? startTime = null)
    {
        if (mode == CountdownMode.Second && startTime >= endTime)
        {
            throw new InvalidOperationException($"{StartTime} 应早于 {EndTime}");
        }
        if (mode != CountdownMode.Second && startTime is not null)
        {
            throw new InvalidOperationException($"仅当 {Mode} 为 {CountdownMode.Second} 才有 {nameof(StartTime)} 字段");
        }
        
        Mode = mode;
        _endTime = endTime;
        _startTime = startTime;
    }
    
    public DateTimeOffset EndTime
    {
        get => _endTime;
        internal set
        {
            if (value <= DateTimeOffset.Now)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(EndTime)} 不能小于服务器当前时间戳");
            }
            _endTime = value;
        }
    }

    public DateTimeOffset? StartTime
    {
        get => _startTime;
        internal set
        {
            if (value is null)
            {
                _startTime = value;
                return;
            }
            if (value <= DateTimeOffset.Now)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(StartTime)} 不能小于服务器当前时间戳");
            }
            _startTime = value;
        }
    }

    public CountdownMode Mode { get; internal set; }
}