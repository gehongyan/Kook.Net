namespace Kook.Net.Docs.Mcp.Source;

/// <summary>
///     按仓库相对路径读取到的一段源码文件内容及其定位信息。
/// </summary>
public sealed class SourceFileResult
{
    /// <summary>仓库相对路径，如 <c>src/Kook.Net.WebSocket/.../SocketGuild.cs</c>。</summary>
    public required string RepositoryPath { get; init; }

    /// <summary>返回内容的起始行号（1 基，含）。</summary>
    public required int StartLine { get; init; }

    /// <summary>返回内容的结束行号（1 基，含）。</summary>
    public required int EndLine { get; init; }

    /// <summary>文件的总行数。</summary>
    public required int TotalLines { get; init; }

    /// <summary>源码文本。</summary>
    public required string Code { get; init; }

    /// <summary>返回内容是否因达到最大行数上限而被截断（仍有后续行未返回）。</summary>
    public bool Truncated { get; init; }
}
