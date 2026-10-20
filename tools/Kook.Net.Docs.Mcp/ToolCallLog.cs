using Microsoft.Extensions.Logging;

namespace Kook.Net.Docs.Mcp;

/// <summary>
///     MCP 工具调用相关日志的源生成器消息定义。
/// </summary>
/// <remarks>
///     日志内容约定为 <c>[MCP] 消息 [字段]{值}</c>：通道前缀 <c>[MCP]</c> 标识来源，
///     字段值同时作为结构化属性。成功为 Information，失败为 Error。
///     仅记录工具名、参数摘要与耗时等非敏感信息。
/// </remarks>
internal static partial class ToolCallLog
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "[MCP] 工具调用开始 [Tool] {Tool} [Args] {Args}")]
    public static partial void CallStarted(ILogger logger, string tool, string args);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Information,
        Message = "[MCP] 工具调用完成 [Tool] {Tool} [IsError] {IsError} [ElapsedMs] {ElapsedMs}")]
    public static partial void CallCompleted(ILogger logger, string tool, bool isError, long elapsedMs);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Error,
        Message = "[MCP] 工具调用异常 [Tool] {Tool} [ElapsedMs] {ElapsedMs}")]
    public static partial void CallFailed(ILogger logger, Exception exception, string tool, long elapsedMs);
}
