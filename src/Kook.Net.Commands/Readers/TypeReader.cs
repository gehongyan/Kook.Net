namespace Kook.Commands;

/// <summary>
///     表示一个将用户的字符串输入解析为指定类型的类型读取器基类。
/// </summary>
public abstract class TypeReader
{
    /// <summary>
    ///     尝试将字符串输入解析为指定类型。
    /// </summary>
    /// <param name="context"> 命令执行上下文。 </param>
    /// <param name="input"> 要解析的字符串输入。 </param>
    /// <param name="services"> 依赖注入服务容器。 </param>
    /// <returns> 一个表示异步解析操作的任务。任务的结果包含解析的结果。 </returns>
    public abstract Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services);
}
