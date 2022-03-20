namespace KaiHeiLa;

/// <summary>
///     文字频道
/// </summary>
public interface ITextChannel : INestedChannel, IMentionable, IMessageChannel
{
    #region General

    /// <summary>
    ///     频道简介
    /// </summary>
    string Topic { get; }
    
    /// <summary>
    ///     慢速模式下限制发言的最短时间间隔, 单位为秒(s)
    /// </summary>
    int SlowModeInterval { get; }

    #endregion
}