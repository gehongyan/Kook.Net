using System.Diagnostics;

namespace Kook;

/// <summary>
///     提供有关 Kook.Net 调试器的能力。
/// </summary>
[DebuggerNonUserCode]
[DebuggerStepThrough]
public static class KookDebugger
{
    private static readonly Action<string> defaultDebugger = message =>
    {
        if (Debugger.IsAttached)
            Debug.WriteLine(message);
        else
            Console.WriteLine(message);
    };

    private static readonly Action<string> emptyDebugger = _ => { };

    private static Action<string> restDebugger = emptyDebugger;
    private static Action<string> packetDebugger = emptyDebugger;
    private static Action<string> ratelimitDebugger = emptyDebugger;
    private static Action<string> audioDebugger = emptyDebugger;

    /// <summary>
    ///     获取或设置一个值，指示是否正在调试 Rest 请求。
    /// </summary>
    public static bool IsDebuggingRest { get; private set; }

    /// <summary>
    ///     获取或设置一个值，指示是否正在调试网关数据包。
    /// </summary>
    public static bool IsDebuggingPacket { get; private set; }

    /// <summary>
    ///     获取或设置一个值，指示是否正在调试速率限制。
    /// </summary>
    public static bool IsDebuggingRatelimit { get; private set; }

    /// <summary>
    ///     获取或设置一个值，指示是否正在调试音频。
    /// </summary>
    public static bool IsDebuggingAudio { get; private set; }

    /// <summary>
    ///     启用所有 Kook.Net 调试器。
    /// </summary>
    /// <remarks>
    ///     当调试器附加到进程上时，Kook.Net 调试器日志将输出调试信息到调试器窗口，否则将输出到控制台。
    /// </remarks>
    public static void EnableAll()
    {
        EnableRest();
        EnablePacket();
        EnableRatelimit();
        EnableAudio();
    }

    /// <summary>
    ///     启用所有 Kook.Net 调试器。
    /// </summary>
    /// <param name="debugger"> 一个接受调试信息类型及字符串参数的调试器委托，用于处理调试信息。 </param>
    public static void EnableAll(Action<KookDebuggerMessageSource, string> debugger)
    {
        EnableRest(message => debugger(KookDebuggerMessageSource.Rest, message));
        EnablePacket(message => debugger(KookDebuggerMessageSource.Packet, message));
        EnableRatelimit(message => debugger(KookDebuggerMessageSource.Ratelimit, message));
        EnableAudio(message => debugger(KookDebuggerMessageSource.Audio, message));
    }

    /// <summary>
    ///     禁用所有 Kook.Net 调试器。
    /// </summary>
    public static void DisableAll()
    {
        DisableRest();
        DisablePacket();
        DisableRatelimit();
        DisableAudio();
    }

    /// <summary>
    ///     启用 Rest 调试器。
    /// </summary>
    /// <remarks>
    ///     当调试器附加到进程上时，Kook.Net 调试器日志将输出调试信息到调试器窗口，否则将输出到控制台。
    /// </remarks>
    public static void EnableRest() => EnableRest(defaultDebugger);

    /// <summary>
    ///     启用 Rest 调试器。
    /// </summary>
    /// <param name="debugger"> 一个接受字符串参数的调试器委托，用于处理调试信息。 </param>
    public static void EnableRest(Action<string> debugger)
    {
        IsDebuggingRest = true;
        restDebugger = debugger;
    }

    /// <summary>
    ///     禁用 Rest 调试器。
    /// </summary>
    public static void DisableRest()
    {
        IsDebuggingRest = false;
        restDebugger = emptyDebugger;
    }

    /// <summary>
    ///     启用网关数据包调试器。
    /// </summary>
    /// <remarks>
    ///     当调试器附加到进程上时，Kook.Net 调试器日志将输出调试信息到调试器窗口，否则将输出到控制台。
    /// </remarks>
    public static void EnablePacket() => EnablePacket(defaultDebugger);

    /// <summary>
    ///     启用网关数据包调试器。
    /// </summary>
    /// <param name="debugger"> 一个接受字符串参数的调试器委托，用于处理调试信息。 </param>
    public static void EnablePacket(Action<string> debugger)
    {
        IsDebuggingPacket = true;
        packetDebugger = debugger;
    }

    /// <summary>
    ///     禁用网关数据包调试器。
    /// </summary>
    public static void DisablePacket()
    {
        IsDebuggingPacket = false;
        packetDebugger = emptyDebugger;
    }

    /// <summary>
    ///     启用速率限制调试器。
    /// </summary>
    /// <remarks>
    ///     当调试器附加到进程上时，Kook.Net 调试器日志将输出调试信息到调试器窗口，否则将输出到控制台。
    /// </remarks>
    public static void EnableRatelimit() => EnableRatelimit(defaultDebugger);

    /// <summary>
    ///     启用速率限制调试器。
    /// </summary>
    /// <param name="debugger"> 一个接受字符串参数的调试器委托，用于处理调试信息。 </param>
    public static void EnableRatelimit(Action<string> debugger)
    {
        IsDebuggingRatelimit = true;
        ratelimitDebugger = debugger;
    }

    /// <summary>
    ///     禁用速率限制调试器。
    /// </summary>
    public static void DisableRatelimit()
    {
        IsDebuggingRatelimit = false;
        ratelimitDebugger = emptyDebugger;
    }

    /// <summary>
    ///     启用音频调试器。
    /// </summary>
    /// <remarks>
    ///     当调试器附加到进程上时，Kook.Net 调试器日志将输出调试信息到调试器窗口，否则将输出到控制台。
    /// </remarks>
    public static void EnableAudio() => EnableAudio(defaultDebugger);

    /// <summary>
    ///     启用音频调试器。
    /// </summary>
    /// <param name="debugger"> 一个接受字符串参数的调试器委托，用于处理调试信息。 </param>
    public static void EnableAudio(Action<string> debugger)
    {
        IsDebuggingAudio = true;
        audioDebugger = debugger;
    }

    /// <summary>
    ///     禁用音频调试器。
    /// </summary>
    public static void DisableAudio()
    {
        IsDebuggingAudio = false;
        audioDebugger = emptyDebugger;
    }

    internal static void DebugRest(string message)
    {
        if (IsDebuggingRest) restDebugger(message);
    }

    internal static void DebugPacket(string message)
    {
        if (IsDebuggingPacket) packetDebugger(message);
    }

    internal static void DebugRatelimit(string message)
    {
        if (IsDebuggingRatelimit) ratelimitDebugger(message);
    }

    internal static void DebugAudio(string message)
    {
        if (IsDebuggingAudio) audioDebugger(message);
    }
}
