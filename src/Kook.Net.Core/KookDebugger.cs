using System.Diagnostics;

namespace Kook;

/// <summary>
///     提供有关 Kook.Net 调试器的能力。
/// </summary>
[DebuggerNonUserCode]
[DebuggerStepThrough]
public static class KookDebugger
{
    private static readonly Action<string> defaultDebugger = Console.WriteLine;

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
    ///     设置 Kook.Net 调试器。
    /// </summary>
    /// <param name="rest"> 设置是否启用 Rest 调试器，当为 <c>null</c> 时不更改 Rest 调试器状态。 </param>
    /// <param name="packet"> 设置是否启用网关数据包调试器，当为 <c>null</c> 时不更改网关数据包调试器状态。</param>
    /// <param name="ratelimit"> 设置是否启用速率限制调试器，当为 <c>null</c> 时不更改速率限制调试器状态。</param>
    /// <param name="audio"> 设置是否启用音频调试器，当为 <c>null</c> 时不更改音频调试器状态。</param>
    /// <param name="debugger"> 设置一个接受调试信息类型及字符串参数的调试器委托，用于处理调试信息，当为 <c>null</c> 时，调试日志将输出到控制台。</param>
    public static void SetDebuggers(
        bool? rest = null,
        bool? packet = null,
        bool? ratelimit = null,
        bool? audio = null,
        Action<KookDebuggerMessageSource, string>? debugger = null)
    {
        if (rest.HasValue)
        {
            if (rest.Value)
            {
                if (debugger is not null)
                    EnableRest(x => debugger(KookDebuggerMessageSource.Rest, x));
                else
                    EnableRest();
            }
            else
                DisableRest();
        }

        if (packet.HasValue)
        {
            if (packet.Value)
            {
                if (debugger is not null)
                    EnablePacket(x => debugger(KookDebuggerMessageSource.Packet, x));
                else
                    EnablePacket();
            }
            else
                DisablePacket();
        }

        if (ratelimit.HasValue)
        {
            if (ratelimit.Value)
            {
                if (debugger is not null)
                    EnableRatelimit(x => debugger(KookDebuggerMessageSource.Ratelimit, x));
                else
                    EnableRatelimit();
            }
            else
                DisableRatelimit();
        }

        if (audio.HasValue)
        {
            if (audio.Value)
            {
                if (debugger is not null)
                    EnableAudio(x => debugger(KookDebuggerMessageSource.Audio, x));
                else
                    EnableAudio();
            }
            else
                DisableAudio();
        }
    }

    /// <summary>
    ///     启用所有 Kook.Net 调试器。
    /// </summary>
    /// <remarks>
    ///     Kook.Net 调试器日志将输出到控制台，要输出到其他位置，请使用
    ///     <see cref="Kook.KookDebugger.EnableAll(System.Action{Kook.KookDebuggerMessageSource,System.String})"/>。
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
    ///     Kook.Net 调试器日志将输出到控制台，要输出到其他位置，请使用
    ///     <see cref="Kook.KookDebugger.EnableRest(System.Action{System.String})"/>。
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
    ///     Kook.Net 调试器日志将输出到控制台，要输出到其他位置，请使用
    ///     <see cref="Kook.KookDebugger.EnablePacket(System.Action{System.String})"/>。
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
    ///     Kook.Net 调试器日志将输出到控制台，要输出到其他位置，请使用
    ///     <see cref="Kook.KookDebugger.EnableRatelimit(System.Action{System.String})"/>。
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
    ///     Kook.Net 调试器日志将输出到控制台，要输出到其他位置，请使用
    ///     <see cref="Kook.KookDebugger.EnableAudio(System.Action{System.String})"/>。
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
        if (IsDebuggingRest)
            SafeInvoke(restDebugger, message);
    }

    internal static void DebugPacket(string message)
    {
        if (IsDebuggingPacket)
            SafeInvoke(packetDebugger, message);
    }

    internal static void DebugRatelimit(string message)
    {
        if (IsDebuggingRatelimit)
            SafeInvoke(ratelimitDebugger, message);
    }

    internal static void DebugAudio(string message)
    {
        if (IsDebuggingAudio)
            SafeInvoke(audioDebugger, message);
    }

    private static void SafeInvoke(Action<string> debugger, string message)
    {
        try
        {
            debugger(message);
        }
        catch
        {
            // ignore
        }
    }
}
