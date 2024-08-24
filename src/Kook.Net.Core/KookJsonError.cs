using System.Collections.Immutable;

namespace Kook;

/// <summary>
///     表示一个在执行 API 请求后从 KOOK 接收到的 JSON 数据中解析出的错误。
/// </summary>
public struct KookJsonError
{
    /// <summary>
    ///     获取错误的 JSON 路径。
    /// </summary>
    public string Path { get; }

    /// <summary>
    ///     获取与路径上的特定属性关联的错误集合。
    /// </summary>
    public IReadOnlyCollection<KookError> Errors { get; }

    internal KookJsonError(string path, KookError[] errors)
    {
        Path = path;
        Errors = errors.ToImmutableArray();
    }
}

/// <summary>
///     表示一个 KOOK 返回的错误。
/// </summary>
public struct KookError
{
    /// <summary>
    ///     获取错误的代码。
    /// </summary>
    public string Code { get; }

    /// <summary>
    ///     获取错误的原因。
    /// </summary>
    public string Message { get; }

    internal KookError(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
