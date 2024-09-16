using System.Reflection;

namespace Kook.Commands;

/// <summary>
///     标记指定的参数应有应由指定的 <see cref="Kook.Commands.TypeReader"/> 读取并解析。
/// </summary>
/// <remarks>
///     此特性允许在解析命令参数时为指定的参数指定特定的
///     <see cref="Kook.Commands.TypeReader"/>，可用于在不影响其他命令的情况下使用特定的命令解析器。
///     <br />
///     <note type="warning">
///         标记此特性的类型解析器类型必须继承自 <see cref="Kook.Commands.TypeReader"/>，否则将引发
///         <see cref="System.InvalidOperationException"/> 异常。
///     </note>
/// </remarks>
/// <example>
///     在以下的示例中，<see cref="TimeSpan"/> 类型的 <c>time</c> 参数将由自定义的类型解析器 <c>FriendlyTimeSpanTypeReader</c>
///     解析，而不是由内置的 <see cref="TimeSpanTypeReader"/> 解析。
///     <code language="cs">
///     [Command("time")]
///     public Task GetTimeAsync([OverrideTypeReader(typeof(FriendlyTimeSpanTypeReader))] TimeSpan time)
///         => ReplyTextAsync(time);
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class OverrideTypeReaderAttribute : Attribute
{
    private static readonly TypeInfo TypeReaderTypeInfo = typeof(TypeReader).GetTypeInfo();

    /// <summary>
    ///     获取解析此参数所使用的类型解析器的类型。
    /// </summary>
    public Type TypeReader { get; }

    /// <summary>
    ///     初始化一个 <see cref="OverrideTypeReaderAttribute"/> 类的新实例。
    /// </summary>
    /// <param name="overridenTypeReader"> 解析此参数所使用的类型解析器的类型。 </param>
    /// <exception cref="InvalidOperationException"> 所提供的类型解析器类型不是 <see cref="Kook.Commands.TypeReader"/> 的派生类。 </exception>
    public OverrideTypeReaderAttribute(Type overridenTypeReader)
    {
        if (!TypeReaderTypeInfo.IsAssignableFrom(overridenTypeReader.GetTypeInfo()))
            throw new InvalidOperationException($"{nameof(overridenTypeReader)} must inherit from {nameof(TypeReader)}.");
        TypeReader = overridenTypeReader;
    }
}
