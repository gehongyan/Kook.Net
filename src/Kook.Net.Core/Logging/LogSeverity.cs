namespace Kook;

/// <summary>
///     表示日志消息的严重性。
/// </summary>
public enum LogSeverity
{
    /// <summary>
    ///     记录包含最高严重级别错误的日志，这类错误可能需要立即关注。
    /// </summary>
    Critical = 0,

    /// <summary>
    ///     记录在执行流程因故障而停止时的日志。
    /// </summary>
    Error = 1,

    /// <summary>
    ///     记录在执行流程中出现的异常活动。
    /// </summary>
    Warning = 2,

    /// <summary>
    ///     记录跟踪应用程序的一般流程的日志。
    /// </summary>
    Info = 3,

    /// <summary>
    ///     记录用于在开发过程中进行交互式调查的日志。
    /// </summary>
    Verbose = 4,

    /// <summary>
    ///     记录任何日志，包括最详细的诊断日志。
    /// </summary>
    Debug = 5
}
